using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.UnitTests.Entities;

/// <summary>
/// Contains unit tests for the <see cref="TodoList"/> entity.
/// </summary>
public class TodoListTests
{
    /// <summary>
    /// Verifies that the default values of a new <see cref="TodoList"/> instance are initialized as expected.
    /// </summary>
    [Fact]
    public void Defaults_AreInitialized()
    {
        var list = new TodoList();

        Assert.Equal(Colour.White, list.Colour);
        Assert.NotNull(list.Items);
        Assert.Empty(list.Items);
        Assert.Null(list.Title);
    }

    /// <summary>
    /// Verifies that the properties of <see cref="TodoList"/> can be set and retrieved as expected.
    /// </summary>
    [Fact]
    public void Properties_CanBeSet()
    {
        var list = new TodoList
        {
            Title = "My list",
            Colour = Colour.Red
        };

        list.Items.Add(new TodoItem { Title = "Item 1" });

        Assert.Equal("My list", list.Title);
        Assert.Equal(Colour.Red, list.Colour);
        Assert.Single(list.Items);
    }
}
