namespace Notification.Contracts.Models;

/// <summary>
/// Specifies the priority level for a notification.
/// </summary>
public enum NotificationPriorityV1
{
    /// <summary>
    /// No priority specified.
    /// </summary>
    None = 0,

    /// <summary>
    /// Low priority.
    /// </summary>
    Low = 1,

    /// <summary>
    /// Normal priority.
    /// </summary>
    Normal = 2,

    /// <summary>
    /// High priority.
    /// </summary>
    High = 3,

    /// <summary>
    /// Urgent priority.
    /// </summary>
    Urgent = 4
}
