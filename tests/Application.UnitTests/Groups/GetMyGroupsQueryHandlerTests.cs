#nullable enable
using Application.Groups.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using System.Linq;

namespace Application.UnitTests.Groups;

/// <summary>
/// Unit tests for <see cref="GetMyGroupsQueryHandler"/>.
/// </summary>
public class GetMyGroupsQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the user identifier is missing.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserIdMissing_ThrowsUnauthorizedAccessException()
    {
        await using var context = TestDbContextFactory.Create();
        var user = new TestUser();
        var handler = new GetMyGroupsQueryHandler(context, user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new GetMyGroupsQuery(), CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns only the current user's groups.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsCurrentUserGroups()
    {
        await using var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var user = new TestUser { UserId = userId };

        var group = new Group { Name = "Team", NormalizedName = "TEAM" };
        var otherGroup = new Group { Name = "Other", NormalizedName = "OTHER" };

        context.Users.AddRange(CreateUser(userId), CreateUser(otherUserId));
        context.Groups.AddRange(group, otherGroup);
        context.UserGroups.AddRange(
            new UserGroup { UserId = userId, Group = group },
            new UserGroup { UserId = otherUserId, Group = otherGroup });
        await context.SaveChangesAsync();

        var handler = new GetMyGroupsQueryHandler(context, user);

        var result = await handler.Handle(new GetMyGroupsQuery { Page = 1, Total = 10 }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data!.Items!);
        Assert.Equal(group.Id, result.Data.Items!.First().Id);
    }

    /// <summary>
    /// Creates a valid user entity for test scenarios.
    /// </summary>
    /// <param name="userId">The identifier to assign to the user.</param>
    /// <returns>A configured <see cref="User"/> entity.</returns>
    private static User CreateUser(Guid userId)
    {
        var unique = Guid.NewGuid().ToString("N");
        return new User
        {
            Id = userId,
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
