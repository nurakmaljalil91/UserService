using System.Net;
using System.Text.Json;
using Application.Common.Exceptions;
using Domain.Common;

namespace WebAPI.Middlewares;

/// <summary>
/// Middleware for handling exceptions globally in the application.
/// </summary>
public sealed class ExceptionHandlingMiddleware: IMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var response = BaseResponse<object>.Fail(
                message: ex.Message,
                errors: ex.Errors);

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, JsonOptions));
        }
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.ContentType = "application/json";

            var response = BaseResponse<object>.Fail(
                message: ex.Message);

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, JsonOptions));
        }
        catch (UnauthorizedAccessException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";

            var response = BaseResponse<object>.Fail(
                message: "Unauthorized.");

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, JsonOptions));
        }
        catch (ForbiddenAccessException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Response.ContentType = "application/json";

            var response = BaseResponse<object>.Fail(
                message: string.IsNullOrWhiteSpace(ex.Message) ? "Forbidden." : ex.Message);

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, JsonOptions));
        }
        catch (BadRequestException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var response = ex.ErrorResponse != null
                ? BaseResponse<object>.Fail(ex.ErrorResponse.Message ?? "Bad request.", ex.ErrorResponse.Errors)
                : BaseResponse<object>.Fail(string.IsNullOrWhiteSpace(ex.Message) ? "Bad request." : ex.Message);

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, JsonOptions));
        }
        catch (Exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = BaseResponse<object>.Fail(
                message: "An unexpected error occurred.");

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, JsonOptions));
        }
    }
}
