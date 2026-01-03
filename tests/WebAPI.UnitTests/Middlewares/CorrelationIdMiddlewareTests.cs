using Microsoft.AspNetCore.Http;
using WebAPI.Middlewares;

namespace WebAPI.UnitTests.Middlewares;

/// <summary>
/// Unit tests for <see cref="CorrelationIdMiddleware"/>.
/// </summary>
public class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_UsesRequestHeaderWhenProvided()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Headers["X-Correlation-ID"] = "corr-123";

        var middleware = new CorrelationIdMiddleware(_ =>
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            return context.Response.StartAsync();
        });

        await middleware.InvokeAsync(context);

        Assert.Equal("corr-123", context.Items["CorrelationId"]);
        Assert.Equal("corr-123", context.Response.Headers["X-Correlation-ID"].ToString());
    }

    [Fact]
    public async Task InvokeAsync_GeneratesCorrelationIdWhenMissing()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new CorrelationIdMiddleware(_ =>
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            return context.Response.StartAsync();
        });

        await middleware.InvokeAsync(context);

        var item = Assert.IsType<string>(context.Items["CorrelationId"]);
        Assert.False(string.IsNullOrWhiteSpace(item));
        Assert.Equal(item, context.Response.Headers["X-Correlation-ID"].ToString());
    }
}
