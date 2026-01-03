using System.Net;
using System.Text.Json;
using Application.Common.Exceptions;
using Domain.Common;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using WebAPI.Middlewares;

namespace WebAPI.UnitTests.Middlewares;

/// <summary>
/// Unit tests for <see cref="ExceptionHandlingMiddleware"/>.
/// </summary>
public class ExceptionHandlingMiddlewareTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task InvokeAsync_HandlesValidationException()
    {
        var failures = new[]
        {
            new ValidationFailure("Title", "Title is required.")
        };
        var middleware = new ExceptionHandlingMiddleware();
        var context = CreateContext();

        await middleware.InvokeAsync(context, _ => throw new ValidationException(failures));

        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        var response = await ReadResponse<BaseResponse<object>>(context);
        Assert.Equal("One or more validation failures have occurred.", response.Message);
        Assert.True(response.Errors!.ContainsKey("Title"));
    }

    [Fact]
    public async Task InvokeAsync_HandlesNotFoundException()
    {
        var middleware = new ExceptionHandlingMiddleware();
        var context = CreateContext();

        await middleware.InvokeAsync(context, _ => throw new NotFoundException("Missing"));

        Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
        var response = await ReadResponse<BaseResponse<object>>(context);
        Assert.Equal("Missing", response.Message);
    }

    [Fact]
    public async Task InvokeAsync_HandlesForbiddenAccessException()
    {
        var middleware = new ExceptionHandlingMiddleware();
        var context = CreateContext();

        await middleware.InvokeAsync(context, _ => throw new ForbiddenAccessException());

        Assert.Equal((int)HttpStatusCode.Forbidden, context.Response.StatusCode);
        var response = await ReadResponse<BaseResponse<object>>(context);
        Assert.Equal("Forbidden.", response.Message);
    }

    [Fact]
    public async Task InvokeAsync_HandlesUnknownException()
    {
        var middleware = new ExceptionHandlingMiddleware();
        var context = CreateContext();

        await middleware.InvokeAsync(context, _ => throw new InvalidOperationException("boom"));

        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        var response = await ReadResponse<BaseResponse<object>>(context);
        Assert.Equal("An unexpected error occurred.", response.Message);
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
