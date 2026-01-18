#nullable enable
using Application.Common.Exceptions;
using Application.Groups.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Groups;

/// <summary>
/// Unit tests for <see cref="UpdateGroupCommandHandler"/>.
/// </summary>
public class UpdateGroupCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the group does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenGroupMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new UpdateGroupCommandHandler(context);

        var command = new UpdateGroupCommand
        {
            Id = Guid.NewGuid(),
            Name = "group.read"
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the name already exists.
    /// </summary>
    [Fact]
    public async Task Handle_WhenNameExists_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var existing = new Group { Name = "staff", NormalizedName = "STAFF" };
        var duplicate = new Group { Name = "admin", NormalizedName = "ADMIN" };
        context.Groups.AddRange(existing, duplicate);
        await context.SaveChangesAsync();

        var handler = new UpdateGroupCommandHandler(context);

        var result = await handler.Handle(new UpdateGroupCommand
        {
            Id = existing.Id,
            Name = "admin"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Group name already exists.", result.Message);
    }

    /// <summary>
    /// Ensures the handler updates the group when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_UpdatesGroup_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var group = new Group { Name = "staff", NormalizedName = "STAFF" };
        context.Groups.Add(group);
        await context.SaveChangesAsync();

        var handler = new UpdateGroupCommandHandler(context);

        var result = await handler.Handle(new UpdateGroupCommand
        {
            Id = group.Id,
            Name = "staff.updated",
            Description = "Updated"
        }, CancellationToken.None);

        Assert.True(result.Success);

        var updated = await context.Groups.SingleAsync(g => g.Id == group.Id);
        Assert.Equal("staff.updated", updated.Name);
        Assert.Equal("STAFF.UPDATED", updated.NormalizedName);
        Assert.Equal("Updated", updated.Description);
    }
}
