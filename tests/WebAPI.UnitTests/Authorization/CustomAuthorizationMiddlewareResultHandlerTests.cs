using System.Text.Json;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using WebAPI.Authorization;

namespace WebAPI.UnitTests.Authorization;

/// <summary>
/// Unit tests for <see cref="CustomAuthorizationMiddlewareResultHandler"/>.
/// </summary>
public class CustomAuthorizationMiddlewareResultHandlerTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task HandleAsync_WritesForbiddenResponse()
    {
        var handler = new CustomAuthorizationMiddlewareResultHandler();
        var context = CreateContext();
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

        await handler.HandleAsync(_ => Task.CompletedTask, context, policy, PolicyAuthorizationResult.Forbid());

        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
        var response = await ReadResponse<BaseResponse<object>>(context);
        Assert.Equal("Forbidden.", response.Message);
    }

    [Fact]
    public async Task HandleAsync_WritesUnauthorizedResponse()
    {
        var handler = new CustomAuthorizationMiddlewareResultHandler();
        var context = CreateContext();
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

        await handler.HandleAsync(_ => Task.CompletedTask, context, policy, PolicyAuthorizationResult.Challenge());

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        var response = await ReadResponse<BaseResponse<object>>(context);
        Assert.Equal("Unauthorized.", response.Message);
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<T> ReadResponse<T>(HttpContext context)
    {
        context.Response.Body.Position = 0;
        return (await JsonSerializer.DeserializeAsync<T>(context.Response.Body, JsonOptions))!;
    }
}
