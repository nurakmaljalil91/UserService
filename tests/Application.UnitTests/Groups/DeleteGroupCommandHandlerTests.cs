#nullable enable
using Application.Common.Exceptions;
using Application.Groups.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Groups;

/// <summary>
/// Unit tests for <see cref="DeleteGroupCommandHandler"/>.
/// </summary>
public class DeleteGroupCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the group does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenGroupMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new DeleteGroupCommandHandler(context);

        var command = new DeleteGroupCommand
        {
            Id = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler deletes the group when it exists.
    /// </summary>
    [Fact]
    public async Task Handle_DeletesGroup_WhenExists()
    {
        await using var context = TestDbContextFactory.Create();
        var group = new Group { Name = "staff", NormalizedName = "STAFF" };
        context.Groups.Add(group);
        await context.SaveChangesAsync();

        var handler = new DeleteGroupCommandHandler(context);

        var result = await handler.Handle(new DeleteGroupCommand
        {
            Id = group.Id
        }, CancellationToken.None);

        Assert.True(result.Success);
        var remaining = await context.Groups.CountAsync();
        Assert.Equal(0, remaining);
    }
}
