using System.Text.Json;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace WebAPI.Authorization;

/// <summary>
/// Formats authorization failures as <see cref="BaseResponse{T}"/>.
/// </summary>
public sealed class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Challenged || authorizeResult.Forbidden)
        {
            context.Response.StatusCode = authorizeResult.Forbidden
                ? StatusCodes.Status403Forbidden
                : StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var message = authorizeResult.Forbidden ? "Forbidden." : "Unauthorized.";
            var response = BaseResponse<object>.Fail(message);

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
            return;
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
