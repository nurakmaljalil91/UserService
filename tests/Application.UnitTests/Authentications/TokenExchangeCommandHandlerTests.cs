#nullable enable
using System.IdentityModel.Tokens.Jwt;
using Application.Authentications.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using NodaTime;

namespace Application.UnitTests.Authentications;

/// <summary>
/// Unit tests for <see cref="TokenExchangeCommandHandler"/>.
/// </summary>
public class TokenExchangeCommandHandlerTests
{
    private const string Secret = "cxs_pat_test-secret";
    private static readonly TestClockService Clock = new();
    private static readonly TestDateTime DateTime = new();

    /// <summary>
    /// Verifies that a valid token yields a short-lived JWT with the token's scopes, the user's roles and PAT markers.
    /// </summary>
    [Fact]
    public async Task Handle_ValidToken_IssuesScopedShortLivedJwt()
    {
        await using var context = TestDbContextFactory.Create();
        var token = await SeedAsync(context, scopes: ["planner:write"]);

        var result = await CreateHandler(context).Handle(new TokenExchangeCommand { Token = Secret }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(new[] { "planner:read", "planner:write" }, result.Data!.Scopes);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.Data.Token);
        Assert.Equal(new[] { "planner:read", "planner:write" }, jwt.Claims.Where(c => c.Type == "scope").Select(c => c.Value));
        Assert.Equal("pat", jwt.Claims.Single(c => c.Type == "token_use").Value);
        Assert.Equal(token.Id.ToString(), jwt.Claims.Single(c => c.Type == "pat_id").Value);
        Assert.Equal("Admin", jwt.Claims.Single(c => c.Type == "role").Value);
        Assert.Equal(token.UserId.ToString(), jwt.Subject);
        Assert.True(result.Data.ExpiresAt <= DateTime.UtcNow.AddMinutes(15));
        Assert.Equal(Clock.Now, token.LastUsedAt);
    }

    /// <summary>
    /// Verifies that unknown, revoked and expired tokens, and tokens of locked users, fail with the same message.
    /// </summary>
    [Theory]
    [InlineData("unknown")]
    [InlineData("revoked")]
    [InlineData("expired")]
    [InlineData("locked")]
    public async Task Handle_UnusableToken_FailsWithGenericMessage(string state)
    {
        await using var context = TestDbContextFactory.Create();
        var token = await SeedAsync(context, scopes: ["planner:read"]);
        switch (state)
        {
            case "revoked": token.Revoke(Clock.Now - Duration.FromMinutes(1)); break;
            case "expired": token.ExpiresAt = Clock.Now; break;
            case "locked": token.User!.IsLocked = true; break;
        }
        await context.SaveChangesAsync();

        var result = await CreateHandler(context).Handle(
            new TokenExchangeCommand { Token = state == "unknown" ? "cxs_pat_nope" : Secret },
            CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(TokenExchangeCommandHandler.InvalidTokenMessage, result.Message);
        Assert.Null(result.Data);
    }

    /// <summary>
    /// Verifies that last-used time is not rewritten on every exchange.
    /// </summary>
    [Fact]
    public async Task Handle_RecentlyUsedToken_DoesNotRewriteLastUsed()
    {
        await using var context = TestDbContextFactory.Create();
        var token = await SeedAsync(context, scopes: ["planner:read"]);
        var recent = Clock.Now - Duration.FromSeconds(20);
        token.LastUsedAt = recent;
        await context.SaveChangesAsync();

        await CreateHandler(context).Handle(new TokenExchangeCommand { Token = Secret }, CancellationToken.None);

        Assert.Equal(recent, token.LastUsedAt);
    }

    /// <summary>
    /// Verifies that interactive refresh-issued tokens carry no PAT markers or scopes.
    /// </summary>
    [Fact]
    public async Task InteractiveTokens_CarryNoScopes()
    {
        await using var context = TestDbContextFactory.Create();
        var token = await SeedAsync(context, scopes: ["planner:read"]);
        context.Sessions.Add(new Session
        {
            UserId = token.UserId,
            RefreshToken = new TestRefreshTokenHasher().Hash("refresh"),
            ExpiresAt = Clock.Now + Duration.FromDays(1)
        });
        await context.SaveChangesAsync();

        var result = await new RefreshTokenCommandHandler(context, DateTime, Clock, Configuration(), new TestRefreshTokenHasher())
            .Handle(new RefreshTokenCommand { RefreshToken = "refresh" }, CancellationToken.None);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.Data!.Token);
        Assert.DoesNotContain(jwt.Claims, c => c.Type is "scope" or "token_use" or "pat_id");
    }

    private static async Task<PersonalAccessToken> SeedAsync(TestApplicationDbContext context, List<string> scopes)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "patuser",
            NormalizedUsername = "PATUSER",
            Email = "patuser@example.com",
            NormalizedEmail = "PATUSER@EXAMPLE.COM"
        };
        var role = new Role { Name = "Admin", NormalizedName = "ADMIN" };
        context.Users.Add(user);
        context.Roles.Add(role);
        context.UserRoles.Add(new UserRole { UserId = user.Id, Role = role });
        var token = new PersonalAccessToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Name = "Claude",
            DisplayPrefix = Secret[..12],
            TokenHash = new TestRefreshTokenHasher().Hash(Secret),
            Scopes = scopes,
            ExpiresAt = Clock.Now + Duration.FromDays(30)
        };
        context.PersonalAccessTokens.Add(token);
        await context.SaveChangesAsync();
        return token;
    }

    private static TokenExchangeCommandHandler CreateHandler(TestApplicationDbContext context)
        => new(context, DateTime, Clock, Configuration(), new TestRefreshTokenHasher());

    private static IConfiguration Configuration() => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "Application.UnitTests",
            ["Jwt:Audience"] = "Application.UnitTests",
            ["Jwt:Key"] = "application-unit-tests-super-secret-key-1234567890",
            ["Jwt:ExpiryMinutes"] = "60"
        })
        .Build();
}
