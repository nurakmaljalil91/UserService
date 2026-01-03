using System;
using System.Collections.Generic;
using System.Text;

namespace Mediator;

/// <summary>
/// Mediator interface to send requests and publish notifications.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Sends a request to the appropriate handler and returns a response.
    /// </summary>
    /// <typeparam name="TResponse">The type of response expected from the request.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a response of type <typeparamref name="TResponse"/>.</returns>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a notification to all relevant handlers.
    /// </summary>
    /// <param name="notification">The notification to publish.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous publish operation.</returns>
    Task Publish(INotification notification, CancellationToken cancellationToken = default);
}
