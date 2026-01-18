#nullable enable
using Application.Common.Exceptions;
using Application.Groups.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Groups;

/// <summary>
/// Unit tests for <see cref="AssignRoleToGroupCommandHandler"/>.
/// </summary>
public class AssignRoleToGroupCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the group does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenGroupMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new AssignRoleToGroupCommandHandler(context);

        var command = new AssignRoleToGroupCommand
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

        var handler = new AssignRoleToGroupCommandHandler(context);

        var command = new AssignRoleToGroupCommand
        {
            GroupId = group.Id,
            RoleId = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the role is already assigned.
    /// </summary>
    [Fact]
    public async Task Handle_WhenRoleAlreadyAssigned_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var group = CreateGroup();
        var role = CreateRole();
        context.Groups.Add(group);
        context.Roles.Add(role);
        context.GroupRoles.Add(new GroupRole { GroupId = group.Id, RoleId = role.Id });
        await context.SaveChangesAsync();

        var handler = new AssignRoleToGroupCommandHandler(context);

        var result = await handler.Handle(new AssignRoleToGroupCommand
        {
            GroupId = group.Id,
            RoleId = role.Id
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Role already assigned.", result.Message);
    }

    /// <summary>
    /// Ensures the handler assigns the role when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_AssignsRole_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var group = CreateGroup();
        var role = CreateRole();
        context.Groups.Add(group);
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var handler = new AssignRoleToGroupCommandHandler(context);

        var result = await handler.Handle(new AssignRoleToGroupCommand
        {
            GroupId = group.Id,
            RoleId = role.Id
        }, CancellationToken.None);

        Assert.True(result.Success);

        var updatedGroup = await context.Groups
            .Include(g => g.GroupRoles)
            .SingleAsync(g => g.Id == group.Id);
        Assert.Single(updatedGroup.GroupRoles);
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
