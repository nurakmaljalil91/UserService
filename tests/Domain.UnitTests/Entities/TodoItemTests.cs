using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.UnitTests.Entities;

/// <summary>
/// Contains unit tests for the <see cref="TodoItem"/> entity.
/// </summary>
public class TodoItemTests
{
    /// <summary>
    /// Verifies that a new <see cref="TodoItem"/> has its default property values initialized as expected.
    /// </summary>
    [Fact]
    public void Defaults_AreInitialized()
    {
        var item = new TodoItem();

        Assert.Equal(0, item.ListId);
        Assert.Null(item.Title);
        Assert.Null(item.Note);
        Assert.Equal(PriorityLevel.None, item.Priority);
        Assert.Null(item.Reminder);
        Assert.Null(item.List);
    }

    /// <summary>
    /// Verifies that the properties of <see cref="TodoItem"/> can be set and retrieved as expected.
    /// </summary>
    [Fact]
    public void Properties_CanBeSet()
    {
        var list = new TodoList { Title = "List", Colour = Colour.Green };
        var reminder = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var item = new TodoItem
        {
            ListId = 1,
            Title = "Title",
            Note = "Note",
            Priority = PriorityLevel.High,
            Reminder = reminder,
            List = list
        };

        Assert.Equal(1, item.ListId);
        Assert.Equal("Title", item.Title);
        Assert.Equal("Note", item.Note);
        Assert.Equal(PriorityLevel.High, item.Priority);
        Assert.Equal(reminder, item.Reminder);
        Assert.Same(list, item.List);
    }
}
