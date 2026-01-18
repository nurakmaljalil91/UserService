#nullable enable
using Application.Common.Exceptions;
using Application.Roles.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Roles;

/// <summary>
/// Unit tests for <see cref="AssignPermissionToRoleCommandHandler"/>.
/// </summary>
public class AssignPermissionToRoleCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the role does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenRoleMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new AssignPermissionToRoleCommandHandler(context);

        var command = new AssignPermissionToRoleCommand
        {
            RoleId = Guid.NewGuid(),
            PermissionId = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler throws when the permission does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenPermissionMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var role = new Role { Name = "role", NormalizedName = "ROLE" };
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var handler = new AssignPermissionToRoleCommandHandler(context);

        var command = new AssignPermissionToRoleCommand
        {
            RoleId = role.Id,
            PermissionId = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the permission is already assigned.
    /// </summary>
    [Fact]
    public async Task Handle_WhenAlreadyAssigned_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var role = new Role { Name = "role", NormalizedName = "ROLE" };
        var permission = new Permission { Name = "perm", NormalizedName = "PERM" };
        context.Roles.Add(role);
        context.Permissions.Add(permission);
        context.RolePermissions.Add(new RolePermission
        {
            RoleId = role.Id,
            PermissionId = permission.Id
        });
        await context.SaveChangesAsync();

        var handler = new AssignPermissionToRoleCommandHandler(context);

        var result = await handler.Handle(new AssignPermissionToRoleCommand
        {
            RoleId = role.Id,
            PermissionId = permission.Id
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Permission already assigned.", result.Message);
    }

    /// <summary>
    /// Ensures the handler assigns the permission when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_AssignsPermission_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var role = new Role { Name = "role", NormalizedName = "ROLE" };
        var permission = new Permission { Name = "perm", NormalizedName = "PERM" };
        context.Roles.Add(role);
        context.Permissions.Add(permission);
        await context.SaveChangesAsync();

        var handler = new AssignPermissionToRoleCommandHandler(context);

        var result = await handler.Handle(new AssignPermissionToRoleCommand
        {
            RoleId = role.Id,
            PermissionId = permission.Id
        }, CancellationToken.None);

        Assert.True(result.Success);

        var updatedRole = await context.Roles
            .Include(r => r.RolePermissions)
            .SingleAsync(r => r.Id == role.Id);
        Assert.Single(updatedRole.RolePermissions);
    }
}
