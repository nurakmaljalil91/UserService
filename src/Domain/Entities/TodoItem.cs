#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities;

/// <summary>
/// Represents a to-do item within a to-do list, including details such as title, note, priority, reminder, and completion status.
/// </summary>
public class TodoItem : BaseAuditableEntity
{
    /// <summary>
    /// Gets or sets the identifier of the to-do list to which this item belongs.
    /// </summary>
    public long ListId { get; set; }

    /// <summary>
    /// Gets or sets the title of the to-do item.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets an optional note or comment associated with the object.
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Gets or sets the priority level assigned to the item.
    /// </summary>
    public PriorityLevel Priority { get; set; }

    /// <summary>
    /// Gets or sets the date and time when a reminder is scheduled to occur.
    /// </summary>
    public DateTime? Reminder { get; set; }

    /// <summary>
    /// Gets or sets the collection of to-do items associated with this instance.
    /// </summary>
    public TodoList List { get; set; } = null!;
}
