#nullable enable
using Application.Roles.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using System.Linq;

namespace Application.UnitTests.Roles;

/// <summary>
/// Unit tests for <see cref="GetMyRolesQueryHandler"/>.
/// </summary>
public class GetMyRolesQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the user identifier is missing.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserIdMissing_ThrowsUnauthorizedAccessException()
    {
        await using var context = TestDbContextFactory.Create();
        var user = new TestUser();
        var handler = new GetMyRolesQueryHandler(context, user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new GetMyRolesQuery(), CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns only the current user's roles.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsCurrentUserRoles()
    {
        await using var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var user = new TestUser { UserId = userId };

        var role = new Role { Name = "Member", NormalizedName = "MEMBER" };
        var otherRole = new Role { Name = "Guest", NormalizedName = "GUEST" };

        context.Users.AddRange(CreateUser(userId), CreateUser(otherUserId));
        context.Roles.AddRange(role, otherRole);
        context.UserRoles.AddRange(
            new UserRole { UserId = userId, Role = role },
            new UserRole { UserId = otherUserId, Role = otherRole });
        await context.SaveChangesAsync();

        var handler = new GetMyRolesQueryHandler(context, user);

        var result = await handler.Handle(new GetMyRolesQuery { Page = 1, Total = 10 }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data!.Items!);
        Assert.Equal(role.Id, result.Data.Items!.First().Id);
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
