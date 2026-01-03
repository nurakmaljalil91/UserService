using Application.Common.Exceptions;
using Application.TodoItems.Commands;
using Application.UnitTests.TestInfrastructure;

namespace Application.UnitTests.TodoItems;

/// <summary>
/// Unit tests for <see cref="DeleteTodoItemCommandHandler"/>.
/// </summary>
public class DeleteTodoItemCommandHandlerTests
{
    /// <summary>
    /// Tests that <see cref="DeleteTodoItemCommandHandler.Handle"/> throws a <see cref="NotFoundException"/>
    /// when attempting to delete a TodoItem that does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenItemMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new DeleteTodoItemCommandHandler(context);

        var command = new DeleteTodoItemCommand(1);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
