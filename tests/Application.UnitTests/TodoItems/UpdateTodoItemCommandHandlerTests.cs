using Application.Common.Exceptions;
using Application.TodoItems.Commands;
using Application.UnitTests.TestInfrastructure;

namespace Application.UnitTests.TodoItems;

/// <summary>
/// Contains unit tests for <see cref="UpdateTodoItemCommandHandler"/>.
/// </summary>
public class UpdateTodoItemCommandHandlerTests
{
    /// <summary>
    /// Verifies that <see cref="UpdateTodoItemCommandHandler.Handle(UpdateTodoItemComand, CancellationToken)"/> throws a <see cref="NotFoundException"/>
    /// when the specified item does not exist in the database.
    /// </summary>
    public async Task Handle_WhenItemMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new UpdateTodoItemCommandHandler(context);

        var command = new UpdateTodoItemComand
        {
            Id = 1,
            Title = "Updated"
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
