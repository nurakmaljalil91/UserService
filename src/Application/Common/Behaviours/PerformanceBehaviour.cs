using Application.Common.Interfaces;
using Mediator;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Common.Behaviours;

/// <summary>
/// Pipeline behavior that logs a warning if a request takes longer than 500 milliseconds to process.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer;
    private readonly ILogger<PerformanceBehaviour<TRequest, TResponse>> _logger;
    private readonly IUser _user;

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceBehaviour{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="user">The current user service.</param>
    public PerformanceBehaviour(
        ILogger<PerformanceBehaviour<TRequest, TResponse>> logger,
        IUser user)
    {
        _timer = new Stopwatch();

        _logger = logger;
        _user = user;        
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _user.Username ?? string.Empty;

            _logger.LogWarning("Application Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@Request}",
                requestName, elapsedMilliseconds, userId, request);
        }

        return response;
    }
}
