using System;
using System.Collections.Generic;
using System.Text;

namespace Mediator;

/// <summary>
/// Handles a notification of type <typeparamref name="TNotification"/>.
/// </summary>
public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    /// <summary>
    /// Handles the specified notification.
    /// </summary>
    /// <param name="notification">The notification to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}
