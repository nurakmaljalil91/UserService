#nullable enable
using Application.Common.Exceptions;
using Application.Permissions.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Permissions;

/// <summary>
/// Unit tests for <see cref="DeletePermissionCommandHandler"/>.
/// </summary>
public class DeletePermissionCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the permission does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenPermissionMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new DeletePermissionCommandHandler(context);

        var command = new DeletePermissionCommand
        {
            Id = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler deletes the permission when it exists.
    /// </summary>
    [Fact]
    public async Task Handle_DeletesPermission_WhenExists()
    {
        await using var context = TestDbContextFactory.Create();
        var permission = new Permission { Name = "read", NormalizedName = "READ" };
        context.Permissions.Add(permission);
        await context.SaveChangesAsync();

        var handler = new DeletePermissionCommandHandler(context);

        var result = await handler.Handle(new DeletePermissionCommand
        {
            Id = permission.Id
        }, CancellationToken.None);

        Assert.True(result.Success);
        var remaining = await context.Permissions.CountAsync();
        Assert.Equal(0, remaining);
    }
}
