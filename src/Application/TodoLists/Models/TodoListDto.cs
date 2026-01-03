#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using Application.TodoItems.Models;
using Domain.Entities;

namespace Application.TodoLists.Models;

/// <summary>
/// Data Transfer Object (DTO) for a to-do list, including its items.
/// </summary>
public class TodoListDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListDto"/> class with default values.
    /// </summary>
    public TodoListDto()
    {
        Items = Array.Empty<TodoItemDto>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListDto"/> class from a <see cref="TodoList"/> entity.
    /// </summary>
    /// <param name="todoList">The <see cref="TodoList"/> entity to map from.</param>
    public TodoListDto(TodoList todoList)
    {
        Id = todoList.Id;
        Title = todoList.Title;
        Colour = todoList.Colour.ToString();
        Items = todoList.Items is null
            ? Array.Empty<TodoItemDto>()
            : todoList.Items.Select(item => new TodoItemDto(item)).ToArray();
    }

    /// <summary>
    /// Gets the unique identifier of the to-do list.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Gets the title of the to-do list.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// Gets the color associated with the to-do list.
    /// </summary>
    public string? Colour { get; init; }

    /// <summary>
    /// Gets the collection of to-do items in the list.
    /// </summary>
    public IReadOnlyCollection<TodoItemDto> Items { get; init; }
}
#nullable restore
