#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Domain.Entities;

/// <summary>
/// Represents a to-do list containing a collection of <see cref="TodoItem"/>s, a title, and a color.
/// Inherits audit properties from <see cref="BaseAuditableEntity"/>.
/// </summary>
public class TodoList : BaseAuditableEntity
{
    /// <summary>
    /// Gets or sets the title of the to-do list.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the color associated with the to-do list.
    /// </summary>
    public Colour Colour { get; set; } = Colour.White;

    /// <summary>
    /// Gets the collection of to-do items in the list.
    /// </summary>
    public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();
}
