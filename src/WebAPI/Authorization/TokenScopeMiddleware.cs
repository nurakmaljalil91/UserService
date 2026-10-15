using System.Security.Claims;
using System.Text.Json;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Authorization;

/// <summary>
/// Enforces per-service scopes on access tokens exchanged from personal access tokens.
/// Interactive (password-login) tokens carry no <c>token_use=pat</c> claim and are not affected.
/// </summary>
/// <remarks>
/// GET, HEAD and OPTIONS requests require <c>users:read</c> (or <c>users:write</c>); every other method requires
/// <c>users:write</c>. Endpoints marked <see cref="AllowAnonymousAttribute"/> are skipped. Scope checks run in
/// addition to role and ownership rules, never instead of them.
/// </remarks>
public sealed class TokenScopeMiddleware : IMiddleware
{
    /// <summary>
    /// The scope prefix owned by this service.
    /// </summary>
    public const string ServiceScope = "users";

    /// <summary>
    /// The claim that marks an access token as exchanged from a personal access token.
    /// </summary>
    public const string TokenUseClaimType = "token_use";

    /// <summary>
    /// The <see cref="TokenUseClaimType"/> value for personal-access-token-derived tokens.
    /// </summary>
    public const string PersonalAccessTokenUse = "pat";

    /// <summary>
    /// The claim type carrying granted scopes.
    /// </summary>
    public const string ScopeClaimType = "scope";

    /// <summary>
    /// Path prefixes treated as reads regardless of HTTP method (for example SignalR hub negotiation).
    /// </summary>
    public static readonly string[] ReadPathPrefixes = [];

    private const string MappedScopeClaimType = "http://schemas.microsoft.com/identity/claims/scope";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Rejects personal-access-token requests that lack the scope required for this service and HTTP method.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="next">The next middleware in the pipeline.</param>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!IsPersonalAccessToken(context.User) ||
            context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() is not null)
        {
            await next(context);
            return;
        }

        var isRead = HttpMethods.IsGet(context.Request.Method) ||
                     HttpMethods.IsHead(context.Request.Method) ||
                     HttpMethods.IsOptions(context.Request.Method) ||
                     ReadPathPrefixes.Any(prefix => context.Request.Path.StartsWithSegments(prefix, StringComparison.OrdinalIgnoreCase));

        if (HasScope(context.User, isRead))
        {
            await next(context);
            return;
        }

        var required = $"{ServiceScope}:{(isRead ? "read" : "write")}";
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";
        var response = BaseResponse<object>.Fail($"Forbidden. This token is missing the '{required}' scope.");
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }

    private static bool IsPersonalAccessToken(ClaimsPrincipal user)
        => user.Identity?.IsAuthenticated == true &&
           user.HasClaim(TokenUseClaimType, PersonalAccessTokenUse);

    private static bool HasScope(ClaimsPrincipal user, bool isRead)
    {
        var scopes = user.Claims
            .Where(c => c.Type is ScopeClaimType or MappedScopeClaimType)
            .SelectMany(c => c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .ToHashSet(StringComparer.Ordinal);

        return scopes.Contains($"{ServiceScope}:write") ||
               (isRead && scopes.Contains($"{ServiceScope}:read"));
    }
}
