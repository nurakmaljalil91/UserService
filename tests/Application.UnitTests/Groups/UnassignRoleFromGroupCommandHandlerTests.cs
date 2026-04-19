#nullable enable
using Application.Common.Exceptions;
using Application.Groups.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Groups;

/// <summary>
/// Unit tests for <see cref="UnassignRoleFromGroupCommandHandler"/>.
/// </summary>
public class UnassignRoleFromGroupCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the group does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenGroupMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new UnassignRoleFromGroupCommandHandler(context);

        var command = new UnassignRoleFromGroupCommand
        {
            GroupId = Guid.NewGuid(),
            RoleId = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler throws when the role does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenRoleMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var group = CreateGroup();
        context.Groups.Add(group);
        await context.SaveChangesAsync();

        var handler = new UnassignRoleFromGroupCommandHandler(context);

        var command = new UnassignRoleFromGroupCommand
        {
            GroupId = group.Id,
            RoleId = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the role is not assigned.
    /// </summary>
    [Fact]
    public async Task Handle_WhenRoleNotAssigned_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var group = CreateGroup();
        var role = CreateRole();
        context.Groups.Add(group);
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var handler = new UnassignRoleFromGroupCommandHandler(context);

        var result = await handler.Handle(new UnassignRoleFromGroupCommand
        {
            GroupId = group.Id,
            RoleId = role.Id
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Role is not assigned to group.", result.Message);
    }

    /// <summary>
    /// Ensures the handler unassigns the role when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_UnassignsRole_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var group = CreateGroup();
        var role = CreateRole();
        context.Groups.Add(group);
        context.Roles.Add(role);
        context.GroupRoles.Add(new GroupRole { GroupId = group.Id, RoleId = role.Id, Role = role });
        await context.SaveChangesAsync();

        var handler = new UnassignRoleFromGroupCommandHandler(context);

        var result = await handler.Handle(new UnassignRoleFromGroupCommand
        {
            GroupId = group.Id,
            RoleId = role.Id
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.DoesNotContain(role.Name, result.Data!.Roles);

        var updatedGroup = await context.Groups
            .Include(g => g.GroupRoles)
            .SingleAsync(g => g.Id == group.Id);
        Assert.Empty(updatedGroup.GroupRoles);
        Assert.True(await context.Roles.AnyAsync(r => r.Id == role.Id));
    }

    /// <summary>
    /// Creates a valid group entity for test scenarios.
    /// </summary>
    /// <returns>A configured <see cref="Group"/> entity.</returns>
    private static Group CreateGroup()
        => new()
        {
            Name = "group",
            NormalizedName = "GROUP"
        };

    /// <summary>
    /// Creates a valid role entity for test scenarios.
    /// </summary>
    /// <returns>A configured <see cref="Role"/> entity.</returns>
    private static Role CreateRole()
        => new()
        {
            Name = "role",
            NormalizedName = "ROLE"
        };
}
