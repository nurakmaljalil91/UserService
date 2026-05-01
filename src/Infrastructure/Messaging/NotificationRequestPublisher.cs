#nullable enable
using Application.Common.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Contracts.Events;

namespace Infrastructure.Messaging;

/// <summary>
/// Publishes notification requests through MassTransit.
/// </summary>
public sealed class NotificationRequestPublisher : INotificationRequestPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<NotificationRequestPublisher> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationRequestPublisher"/> class.
    /// </summary>
    /// <param name="publishEndpoint">The MassTransit publish endpoint.</param>
    /// <param name="logger">The logger for publish failures.</param>
    public NotificationRequestPublisher(
        IPublishEndpoint publishEndpoint,
        ILogger<NotificationRequestPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task PublishAsync(NotificationRequestedV1 notification, CancellationToken cancellationToken)
    {
        try
        {
            await _publishEndpoint.Publish(notification, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogWarning(
                exception,
                "Failed to publish notification request {SourceEventType}:{SourceEventId}.",
                notification.SourceEventType,
                notification.SourceEventId);
        }
    }
}
