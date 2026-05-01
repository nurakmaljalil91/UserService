#nullable enable
using Notification.Contracts.Events;

namespace Application.Common.Interfaces;

/// <summary>
/// Publishes notification requests to the notification system.
/// </summary>
public interface INotificationRequestPublisher
{
    /// <summary>
    /// Publishes a notification request asynchronously.
    /// </summary>
    /// <param name="notification">The notification request to publish.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishAsync(NotificationRequestedV1 notification, CancellationToken cancellationToken);
}
