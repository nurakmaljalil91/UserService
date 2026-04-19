#nullable enable
using Application.Common.Exceptions;
using Application.Groups.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Groups;

/// <summary>
/// Unit tests for <see cref="UnassignUserFromGroupCommandHandler"/>.
/// </summary>
public class UnassignUserFromGroupCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the group does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenGroupMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new UnassignUserFromGroupCommandHandler(context);

        var command = new UnassignUserFromGroupCommand
        {
            GroupId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler throws when the user does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var group = CreateGroup();
        context.Groups.Add(group);
        await context.SaveChangesAsync();

        var handler = new UnassignUserFromGroupCommandHandler(context);

        var command = new UnassignUserFromGroupCommand
        {
            GroupId = group.Id,
            UserId = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the user is not assigned.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserNotAssigned_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var group = CreateGroup();
        var user = CreateUser();
        context.Groups.Add(group);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new UnassignUserFromGroupCommandHandler(context);

        var result = await handler.Handle(new UnassignUserFromGroupCommand
        {
            GroupId = group.Id,
            UserId = user.Id
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User is not assigned to group.", result.Message);
    }

    /// <summary>
    /// Ensures the handler unassigns the user when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_UnassignsUser_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var group = CreateGroup();
        var user = CreateUser();
        context.Groups.Add(group);
        context.Users.Add(user);
        context.UserGroups.Add(new UserGroup { GroupId = group.Id, UserId = user.Id, User = user });
        await context.SaveChangesAsync();

        var handler = new UnassignUserFromGroupCommandHandler(context);

        var result = await handler.Handle(new UnassignUserFromGroupCommand
        {
            GroupId = group.Id,
            UserId = user.Id
        }, CancellationToken.None);

        Assert.True(result.Success);

        var updatedGroup = await context.Groups
            .Include(g => g.UserGroups)
            .SingleAsync(g => g.Id == group.Id);
        Assert.Empty(updatedGroup.UserGroups);
        Assert.True(await context.Users.AnyAsync(u => u.Id == user.Id));
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
}
