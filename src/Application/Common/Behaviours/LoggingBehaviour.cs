using Mediator;
using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours;

/// <summary>
/// Provides logging behavior for MediatR pipeline requests and responses.
/// Logs request handling start, completion with elapsed time, and errors.
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{

    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly IUser _user;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging request and response information.</param>
    /// <param name="user">The current user service providing user context.</param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, IUser user)
    {
        _logger = logger;
        _user = user;
    }

    /// <summary>
    /// Handles the MediatR pipeline request by logging the start, completion, and any errors encountered.
    /// </summary>
    /// <param name="request">The request instance being handled.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <param name="next">The delegate representing the next handler in the pipeline.</param>
    /// <returns>The response from the next handler in the pipeline.</returns>
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var requestName = typeof(TRequest).Name;    
        var start = DateTime.UtcNow;

        _logger.LogInformation("Handling {RequestName}", requestName);

        try
        {
            var response = await next().ConfigureAwait(false);
            var elapsed = (DateTime.UtcNow - start).TotalMilliseconds;
            _logger.LogInformation("Handled {RequestName} in {Elapsed:0.000} ms", requestName, elapsed);
            return response;
        }
        catch (Exception ex)
        {
            var userId = _user.Username ?? "Unknown";
            _logger.LogError(ex, "Error handling {RequestName} for user {UserId}: {ExceptionMessage}", requestName, userId, ex.Message);
            throw new InvalidOperationException($"Error handling {requestName} for user {userId}.", ex);
        }
    }
}
