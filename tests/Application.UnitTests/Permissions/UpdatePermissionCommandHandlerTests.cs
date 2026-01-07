#nullable enable
using Application.Common.Exceptions;
using Application.Permissions.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Permissions;

/// <summary>
/// Unit tests for <see cref="UpdatePermissionCommandHandler"/>.
/// </summary>
public class UpdatePermissionCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the permission does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenPermissionMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new UpdatePermissionCommandHandler(context);

        var command = new UpdatePermissionCommand
        {
            Id = Guid.NewGuid(),
            Name = "permission.read"
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
        var existing = new Permission { Name = "read", NormalizedName = "READ" };
        var duplicate = new Permission { Name = "write", NormalizedName = "WRITE" };
        context.Permissions.AddRange(existing, duplicate);
        await context.SaveChangesAsync();

        var handler = new UpdatePermissionCommandHandler(context);

        var result = await handler.Handle(new UpdatePermissionCommand
        {
            Id = existing.Id,
            Name = "write"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Permission name already exists.", result.Message);
    }

    /// <summary>
    /// Ensures the handler updates the permission when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_UpdatesPermission_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var permission = new Permission { Name = "read", NormalizedName = "READ" };
        context.Permissions.Add(permission);
        await context.SaveChangesAsync();

        var handler = new UpdatePermissionCommandHandler(context);

        var result = await handler.Handle(new UpdatePermissionCommand
        {
            Id = permission.Id,
            Name = "read.updated",
            Description = "Updated"
        }, CancellationToken.None);

        Assert.True(result.Success);

        var updated = await context.Permissions.SingleAsync(p => p.Id == permission.Id);
        Assert.Equal("read.updated", updated.Name);
        Assert.Equal("READ.UPDATED", updated.NormalizedName);
        Assert.Equal("Updated", updated.Description);
    }
}
