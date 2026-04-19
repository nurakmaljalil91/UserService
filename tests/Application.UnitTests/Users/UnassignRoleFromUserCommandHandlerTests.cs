#nullable enable
using Application.Common.Exceptions;
using Application.UnitTests.TestInfrastructure;
using Application.Users.Commands;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Users;

/// <summary>
/// Unit tests for <see cref="UnassignRoleFromUserCommandHandler"/>.
/// </summary>
public class UnassignRoleFromUserCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the user does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new UnassignRoleFromUserCommandHandler(context);

        var command = new UnassignRoleFromUserCommand
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

        var handler = new UnassignRoleFromUserCommandHandler(context);

        var command = new UnassignRoleFromUserCommand
        {
            UserId = user.Id,
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
        var user = CreateUser();
        var role = CreateRole();
        context.Users.Add(user);
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var handler = new UnassignRoleFromUserCommandHandler(context);

        var result = await handler.Handle(new UnassignRoleFromUserCommand
        {
            UserId = user.Id,
            RoleId = role.Id
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Role is not assigned to user.", result.Message);
    }

    /// <summary>
    /// Ensures the handler unassigns the role when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_UnassignsRole_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        var role = CreateRole();
        context.Users.Add(user);
        context.Roles.Add(role);
        context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id, Role = role });
        await context.SaveChangesAsync();

        var handler = new UnassignRoleFromUserCommandHandler(context);

        var result = await handler.Handle(new UnassignRoleFromUserCommand
        {
            UserId = user.Id,
            RoleId = role.Id
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.DoesNotContain(role.Name, result.Data!.Roles);

        var updatedUser = await context.Users
            .Include(u => u.UserRoles)
            .SingleAsync(u => u.Id == user.Id);
        Assert.Empty(updatedUser.UserRoles);
        Assert.True(await context.Roles.AnyAsync(r => r.Id == role.Id));
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
