using Application.Common.Exceptions;
using Application.TodoLists.Commands;
using Application.UnitTests.TestInfrastructure;

namespace Application.UnitTests.TodoLists;

/// <summary>
/// Unit tests for <see cref="DeleteTodoListCommandHandler"/>.
/// </summary>
public class DeleteTodoListCommandHandlerTests
{
    /// <summary>
    /// Ensures that <see cref="DeleteTodoListCommandHandler.Handle(DeleteTodoListCommand, CancellationToken)"/>
    /// throws a <see cref="NotFoundException"/> when the specified list does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenListMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new DeleteTodoListCommandHandler(context);

        var command = new DeleteTodoListCommand
        {
            Id = 1
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
