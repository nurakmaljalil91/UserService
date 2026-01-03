using Application.Common.Exceptions;
using Application.TodoLists.Commands;
using Application.UnitTests.TestInfrastructure;

namespace Application.UnitTests.TodoLists;

/// <summary>
/// Contains unit tests for <see cref="UpdateTodoListCommandHandler"/>.
/// </summary>
public class UpdateTodoListCommandHandlerTests
{
    /// <summary>
    /// Verifies that <see cref="UpdateTodoListCommandHandler.Handle"/> throws a <see cref="NotFoundException"/>
    /// when the specified todos list does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenListMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new UpdateTodoListCommandHandler(context);

        var command = new UpdateTodoListCommand
        {
            Id = 1,
            Title = "Updated"
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
