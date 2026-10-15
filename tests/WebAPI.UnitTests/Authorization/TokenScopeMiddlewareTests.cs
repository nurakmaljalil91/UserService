#nullable enable
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using WebAPI.Authorization;

namespace WebAPI.UnitTests.Authorization;

/// <summary>
/// Unit tests for <see cref="TokenScopeMiddleware"/>.
/// </summary>
public class TokenScopeMiddlewareTests
{
    private static readonly string Read = $"{TokenScopeMiddleware.ServiceScope}:read";
    private static readonly string Write = $"{TokenScopeMiddleware.ServiceScope}:write";
    private const string OtherWrite = "otherservice:write";

    /// <summary>
    /// Scope/method combinations and whether the request should reach the next middleware.
    /// </summary>
    public static TheoryData<string, string[], bool> PersonalAccessTokenCases()
    {
        var data = new TheoryData<string, string[], bool>
        {
            { "GET", new[] { Read }, true },
            { "HEAD", new[] { Read }, true },
            { "GET", new[] { Write }, true },
            { "POST", new[] { Write }, true },
            { "PATCH", new[] { Write }, true },
            { "DELETE", new[] { Write }, true },
            { "POST", new[] { Read }, false },
            { "DELETE", new[] { Read }, false },
            { "GET", new[] { OtherWrite }, false },
            { "POST", new[] { OtherWrite }, false },
            { "GET", Array.Empty<string>(), false },
            { "POST", new[] { Read, OtherWrite }, false },
        };
        return data;
    }

    /// <summary>
    /// Verifies that personal-access-token requests are allowed only with the matching service scope.
    /// </summary>
    [Theory]
    [MemberData(nameof(PersonalAccessTokenCases))]
    public async Task PersonalAccessToken_IsCheckedAgainstServiceScope(string method, string[] scopes, bool allowed)
    {
        var context = CreateContext(method, PatPrincipal(scopes));
        var reachedNext = false;

        await new TokenScopeMiddleware().InvokeAsync(context, _ =>
        {
            reachedNext = true;
            return Task.CompletedTask;
        });

        Assert.Equal(allowed, reachedNext);
        Assert.Equal(allowed ? StatusCodes.Status200OK : StatusCodes.Status403Forbidden, context.Response.StatusCode);
    }

    /// <summary>
    /// Verifies that scopes are also read from a single space-separated claim.
    /// </summary>
    [Fact]
    public async Task PersonalAccessToken_SpaceSeparatedScopeClaim_IsAccepted()
    {
        var principal = PatPrincipal();
        ((ClaimsIdentity)principal.Identity!).AddClaim(new Claim(TokenScopeMiddleware.ScopeClaimType, $"{OtherWrite} {Write}"));
        var reachedNext = false;

        await new TokenScopeMiddleware().InvokeAsync(CreateContext("POST", principal), _ =>
        {
            reachedNext = true;
            return Task.CompletedTask;
        });

        Assert.True(reachedNext);
    }

    /// <summary>
    /// Verifies that interactive tokens (no token_use claim) and anonymous requests are not affected.
    /// </summary>
    [Theory]
    [InlineData("GET")]
    [InlineData("DELETE")]
    public async Task NonPersonalAccessTokenRequests_PassThrough(string method)
    {
        var interactive = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) }, "Bearer"));
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        var calls = 0;

        await new TokenScopeMiddleware().InvokeAsync(CreateContext(method, interactive), _ => { calls++; return Task.CompletedTask; });
        await new TokenScopeMiddleware().InvokeAsync(CreateContext(method, anonymous), _ => { calls++; return Task.CompletedTask; });

        Assert.Equal(2, calls);
    }

    /// <summary>
    /// Verifies that [AllowAnonymous] endpoints are skipped even for personal access tokens without scopes.
    /// </summary>
    [Fact]
    public async Task AllowAnonymousEndpoint_IsSkipped()
    {
        var context = CreateContext("POST", PatPrincipal());
        context.SetEndpoint(new Endpoint(
            _ => Task.CompletedTask,
            new EndpointMetadataCollection(new AllowAnonymousAttribute()),
            "anonymous"));
        var reachedNext = false;

        await new TokenScopeMiddleware().InvokeAsync(context, _ =>
        {
            reachedNext = true;
            return Task.CompletedTask;
        });

        Assert.True(reachedNext);
    }

    /// <summary>
    /// Verifies that a rejected request gets a JSON body naming the missing scope.
    /// </summary>
    [Fact]
    public async Task Rejection_WritesJsonBodyNamingMissingScope()
    {
        var context = CreateContext("POST", PatPrincipal(Read));

        await new TokenScopeMiddleware().InvokeAsync(context, _ => Task.CompletedTask);

        context.Response.Body.Position = 0;
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.Equal("application/json", context.Response.ContentType);
        Assert.Contains(Write, body);
    }

    /// <summary>
    /// Verifies that configured read paths need only the read scope, whatever the HTTP method.
    /// </summary>
    [Fact]
    public async Task ReadPathPrefixes_RequireOnlyReadScope()
    {
        foreach (var prefix in TokenScopeMiddleware.ReadPathPrefixes)
        {
            var context = CreateContext("POST", PatPrincipal(Read));
            context.Request.Path = $"{prefix}/negotiate";
            var reachedNext = false;

            await new TokenScopeMiddleware().InvokeAsync(context, _ =>
            {
                reachedNext = true;
                return Task.CompletedTask;
            });

            Assert.True(reachedNext, prefix);
        }
    }

    private static ClaimsPrincipal PatPrincipal(params string[] scopes)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new(TokenScopeMiddleware.TokenUseClaimType, TokenScopeMiddleware.PersonalAccessTokenUse)
        };
        claims.AddRange(scopes.Select(s => new Claim(TokenScopeMiddleware.ScopeClaimType, s)));
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
    }

    private static DefaultHttpContext CreateContext(string method, ClaimsPrincipal user)
    {
        var context = new DefaultHttpContext { User = user };
        context.Request.Method = method;
        context.Response.Body = new MemoryStream();
        return context;
    }
}
