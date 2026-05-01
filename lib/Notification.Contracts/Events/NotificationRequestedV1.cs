#nullable enable
using Notification.Contracts.Models;

namespace Notification.Contracts.Events;

/// <summary>
/// Represents a request to create and deliver a notification.
/// </summary>
public record NotificationRequestedV1
{
    /// <summary>
    /// Gets the name of the source service emitting this request.
    /// </summary>
    public string SourceService { get; init; } = string.Empty;

    /// <summary>
    /// Gets the type of source event that triggered this notification.
    /// </summary>
    public string SourceEventType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the identifier of the source event to support idempotency.
    /// </summary>
    public string SourceEventId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the correlation identifier for tracing across services.
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Gets the notification title or subject.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// Gets the notification body content.
    /// </summary>
    public string? Body { get; init; }

    /// <summary>
    /// Gets the template key used to render this notification.
    /// </summary>
    public string? TemplateKey { get; init; }

    /// <summary>
    /// Gets the template version used to render this notification.
    /// </summary>
    public string? TemplateVersion { get; init; }

    /// <summary>
    /// Gets the deep link associated with this notification.
    /// </summary>
    public string? DeepLinkUrl { get; init; }

    /// <summary>
    /// Gets the serialized metadata payload for this notification.
    /// </summary>
    public string? MetadataJson { get; init; }

    /// <summary>
    /// Gets the priority level of this notification.
    /// </summary>
    public NotificationPriorityV1 Priority { get; init; } = NotificationPriorityV1.Normal;

    /// <summary>
    /// Gets the time the notification should be processed, in UTC.
    /// </summary>
    public DateTimeOffset? ScheduledForUtc { get; init; }

    /// <summary>
    /// Gets the timestamp for when the request was created, in UTC.
    /// </summary>
    public DateTimeOffset RequestedAtUtc { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets the default channels to use when recipient channels are not specified.
    /// </summary>
    public IReadOnlyCollection<NotificationChannelV1> Channels { get; init; }
        = Array.Empty<NotificationChannelV1>();

    /// <summary>
    /// Gets the recipients to notify.
    /// </summary>
    public IReadOnlyCollection<NotificationRecipientV1> Recipients { get; init; }
        = Array.Empty<NotificationRecipientV1>();
}
