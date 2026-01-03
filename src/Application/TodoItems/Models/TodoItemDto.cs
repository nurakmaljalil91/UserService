#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Domain.Enums;

namespace Application.TodoItems.Models;

/// <summary>
/// Data Transfer Object representing a to-do item.
/// </summary>
public class TodoItemDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the to-do item.
    /// </summary>
    public long Id { get; set; }
    /// <summary>
    /// Gets or sets the identifier of the to-do list to which this item belongs.
    /// </summary>
    public long ListId { get; set; }

    /// <summary>
    /// Gets or sets the title of the to-do item.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets a value indicating whether the to-do item is completed.
    /// </summary>
    public bool Done { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoItemDto"/> class from a <see cref="TodoItem"/> entity.
    /// </summary>
    /// <param name="source">The source <see cref="TodoItem"/> entity.</param>
    public TodoItemDto(TodoItem source)
    {
        Id = source.Id;
        ListId = source.ListId;
        Title = source.Title;
        Done = false;
    }
}
