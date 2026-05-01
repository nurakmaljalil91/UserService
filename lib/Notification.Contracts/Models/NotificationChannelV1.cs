namespace Notification.Contracts.Models;

/// <summary>
/// Specifies the channel through which a notification is delivered.
/// </summary>
public enum NotificationChannelV1
{
    /// <summary>
    /// In-application notification channel.
    /// </summary>
    InApp = 0,

    /// <summary>
    /// Email notification channel.
    /// </summary>
    Email = 1,

    /// <summary>
    /// SMS notification channel.
    /// </summary>
    Sms = 2,

    /// <summary>
    /// WhatsApp notification channel.
    /// </summary>
    WhatsApp = 3,

    /// <summary>
    /// Push notification channel.
    /// </summary>
    Push = 4
}
