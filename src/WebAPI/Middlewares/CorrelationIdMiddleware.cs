using System.Diagnostics;
using Serilog.Context;

namespace WebAPI.Middlewares;

/// <summary>
/// Middleware to handle correlation IDs for incoming requests.
/// </summary>
public sealed class CorrelationIdMiddleware
{
    private const string HeaderKey = "X-Correlation-ID";
    private const string ItemKey = "CorrelationId";

    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationIdMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the request pipeline.</param>
    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Handles the incoming HTTP request, sets or creates a correlation ID,
    /// stores it in the context, adds it to the response header, and enriches Serilog logs.
    /// </summary>
    /// <param name="context">The current HTTP context for the request.</param>
    /// <returns>A task that represents the completion of request processing.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);

        // Store in HttpContext.Items for downstream components
        context.Items[ItemKey] = correlationId;

        // Add to the response header for client visibility
        context.Response.Headers[HeaderKey] = correlationId;

        // Enrich Serilog scope so all logs include the correlation id
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        // Prefer header from caller (BFF/frontend), else reuse TraceIdentifier, else create new GUID.
        if (context.Request.Headers.TryGetValue(HeaderKey, out var correlationId) &&
            !string.IsNullOrWhiteSpace(correlationId))
        {
            return correlationId.ToString();
        }

        return Guid.NewGuid().ToString();
    }
}
