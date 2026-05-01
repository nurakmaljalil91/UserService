#nullable enable
using Application.Common.Interfaces;
using Notification.Contracts.Events;

namespace Application.UnitTests.TestInfrastructure;

/// <summary>
/// Captures notification requests published by application handlers during tests.
/// </summary>
public sealed class TestNotificationRequestPublisher : INotificationRequestPublisher
{
    private readonly List<NotificationRequestedV1> _notifications = new();

    /// <summary>
    /// Gets the notification requests published during the test.
    /// </summary>
    public IReadOnlyList<NotificationRequestedV1> Notifications => _notifications;

    /// <inheritdoc />
    public Task PublishAsync(NotificationRequestedV1 notification, CancellationToken cancellationToken)
    {
        _notifications.Add(notification);
        return Task.CompletedTask;
    }
}
