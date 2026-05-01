namespace Notification.Contracts.Models;

/// <summary>
/// Specifies the type of recipient for a notification.
/// </summary>
public enum RecipientTypeV1
{
    /// <summary>
    /// Recipient type is unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// A single human recipient.
    /// </summary>
    Individual = 1,

    /// <summary>
    /// An organization or business recipient.
    /// </summary>
    Organization = 2,

    /// <summary>
    /// A group of recipients.
    /// </summary>
    Group = 3,

    /// <summary>
    /// System or integration recipient.
    /// </summary>
    System = 4
}
