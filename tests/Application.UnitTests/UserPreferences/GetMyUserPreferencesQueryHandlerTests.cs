#nullable enable
using Application.UnitTests.TestInfrastructure;
using Application.UserPreferences.Queries;
using Domain.Entities;
using System.Linq;

namespace Application.UnitTests.UserPreferences;

/// <summary>
/// Unit tests for <see cref="GetMyUserPreferencesQueryHandler"/>.
/// </summary>
public class GetMyUserPreferencesQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the user identifier is missing.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserIdMissing_ThrowsUnauthorizedAccessException()
    {
        await using var context = TestDbContextFactory.Create();
        var user = new TestUser();
        var handler = new GetMyUserPreferencesQueryHandler(context, user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new GetMyUserPreferencesQuery(), CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns only the current user's preferences.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsCurrentUserPreferences()
    {
        await using var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var user = new TestUser { UserId = userId };

        context.Users.AddRange(CreateUser(userId), CreateUser(otherUserId));
        context.UserPreferences.AddRange(
            new UserPreference { UserId = userId, Key = "theme", Value = "dark" },
            new UserPreference { UserId = otherUserId, Key = "theme", Value = "light" });
        await context.SaveChangesAsync();

        var handler = new GetMyUserPreferencesQueryHandler(context, user);

        var result = await handler.Handle(new GetMyUserPreferencesQuery { Page = 1, Total = 10 }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data!.Items!);
        Assert.Equal(userId, result.Data.Items!.First().UserId);
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
