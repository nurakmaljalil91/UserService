#nullable enable
namespace Notification.Contracts.Models;

/// <summary>
/// Represents recipient details for notification requests.
/// </summary>
public record NotificationRecipientV1
{
    /// <summary>
    /// Gets the recipient identifier from the source system.
    /// </summary>
    public string RecipientId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the recipient type.
    /// </summary>
    public RecipientTypeV1 RecipientType { get; init; } = RecipientTypeV1.Unknown;

    /// <summary>
    /// Gets the recipient display name.
    /// </summary>
    public string? DisplayName { get; init; }

    /// <summary>
    /// Gets the recipient email address.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Gets the recipient phone number for SMS.
    /// </summary>
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Gets the recipient WhatsApp number.
    /// </summary>
    public string? WhatsAppNumber { get; init; }

    /// <summary>
    /// Gets the recipient push token.
    /// </summary>
    public string? PushToken { get; init; }

    /// <summary>
    /// Gets whether in-app notifications are enabled.
    /// </summary>
    public bool InAppEnabled { get; init; } = true;

    /// <summary>
    /// Gets the channels to use for this recipient when provided.
    /// </summary>
    public IReadOnlyCollection<NotificationChannelV1> Channels { get; init; }
        = Array.Empty<NotificationChannelV1>();
}
