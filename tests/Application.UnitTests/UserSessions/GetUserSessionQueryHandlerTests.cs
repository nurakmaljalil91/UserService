#nullable enable
using Application.Common.Exceptions;
using Application.UnitTests.TestInfrastructure;
using Application.UserSessions.Queries;
using Domain.Entities;

namespace Application.UnitTests.UserSessions;

/// <summary>
/// Unit tests for <see cref="GetUserSessionQueryHandler"/>.
/// </summary>
public class GetUserSessionQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the user identifier is missing.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserIdMissing_ThrowsUnauthorizedAccessException()
    {
        await using var context = TestDbContextFactory.Create();
        var user = new TestUser();
        var handler = new GetUserSessionQueryHandler(context, user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new GetUserSessionQuery(), CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler throws when the user does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var user = new TestUser { UserId = userId };
        var handler = new GetUserSessionQueryHandler(context, user);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new GetUserSessionQuery(), CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler throws when the user profile is missing.
    /// </summary>
    [Fact]
    public async Task Handle_WhenProfileMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var user = new TestUser { UserId = userId };

        context.Users.Add(CreateUser(userId));
        await context.SaveChangesAsync();

        var handler = new GetUserSessionQueryHandler(context, user);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new GetUserSessionQuery(), CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns the current user session when it exists.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsCurrentUserSession()
    {
        await using var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var user = new TestUser { UserId = userId };

        context.Users.Add(CreateUser(userId));
        context.UserProfiles.Add(new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DisplayName = "Session User"
        });
        context.UserPreferences.Add(new UserPreference
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Key = "theme",
            Value = "dark"
        });
        await context.SaveChangesAsync();

        var handler = new GetUserSessionQueryHandler(context, user);

        var result = await handler.Handle(new GetUserSessionQuery(), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(userId, result.Data!.User.Id);
        Assert.Equal("Session User", result.Data!.Profile.DisplayName);
        Assert.Single(result.Data!.Preferences);
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
