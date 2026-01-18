#nullable enable
using Application.Common.Exceptions;
using Application.UnitTests.TestInfrastructure;
using Application.Users.Commands;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Users;

/// <summary>
/// Unit tests for <see cref="AssignRoleToUserCommandHandler"/>.
/// </summary>
public class AssignRoleToUserCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the user does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new AssignRoleToUserCommandHandler(context);

        var command = new AssignRoleToUserCommand
        {
            UserId = Guid.NewGuid(),
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
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new AssignRoleToUserCommandHandler(context);

        var command = new AssignRoleToUserCommand
        {
            UserId = user.Id,
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
        var user = CreateUser();
        var role = CreateRole();
        context.Users.Add(user);
        context.Roles.Add(role);
        context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
        await context.SaveChangesAsync();

        var handler = new AssignRoleToUserCommandHandler(context);

        var result = await handler.Handle(new AssignRoleToUserCommand
        {
            UserId = user.Id,
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
        var user = CreateUser();
        var role = CreateRole();
        context.Users.Add(user);
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var handler = new AssignRoleToUserCommandHandler(context);

        var result = await handler.Handle(new AssignRoleToUserCommand
        {
            UserId = user.Id,
            RoleId = role.Id
        }, CancellationToken.None);

        Assert.True(result.Success);

        var updatedUser = await context.Users
            .Include(u => u.UserRoles)
            .SingleAsync(u => u.Id == user.Id);
        Assert.Single(updatedUser.UserRoles);
    }

    /// <summary>
    /// Creates a valid user entity for test scenarios.
    /// </summary>
    /// <returns>A configured <see cref="User"/> entity.</returns>
    private static User CreateUser()
    {
        var unique = Guid.NewGuid().ToString("N");
        return new User
        {
            Username = $"user-{unique}",
            NormalizedUsername = $"USER-{unique}".ToUpperInvariant(),
            Email = $"user-{unique}@example.com",
            NormalizedEmail = $"USER-{unique}@EXAMPLE.COM",
            PasswordHash = "hashed",
            EmailConfirm = false,
            PhoneNumberConfirm = false,
            TwoFactorEnabled = false,
            AccessFailedCount = 0,
            IsLocked = false,
            IsDeleted = false
        };
    }

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
