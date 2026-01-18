#nullable enable
using Application.Common.Exceptions;
using Application.UnitTests.TestInfrastructure;
using Application.UserProfiles.Queries;
using Domain.Entities;

namespace Application.UnitTests.UserProfiles;

/// <summary>
/// Unit tests for <see cref="GetMyUserProfileQueryHandler"/>.
/// </summary>
public class GetMyUserProfileQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the current user identifier is missing.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserIdMissing_ThrowsUnauthorizedAccessException()
    {
        await using var context = TestDbContextFactory.Create();
        var user = new TestUser();
        var handler = new GetMyUserProfileQueryHandler(context, user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new GetMyUserProfileQuery(), CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler throws when the user profile does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenProfileMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var user = new TestUser { UserId = userId };
        var handler = new GetMyUserProfileQueryHandler(context, user);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new GetMyUserProfileQuery(), CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns the current user's profile when it exists.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsUserProfile_WhenExists()
    {
        await using var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var user = new TestUser { UserId = userId };

        var entity = CreateUser(userId);
        var profile = new UserProfile
        {
            UserId = userId,
            DisplayName = "Self Profile"
        };

        context.Users.Add(entity);
        context.UserProfiles.Add(profile);
        await context.SaveChangesAsync();

        var handler = new GetMyUserProfileQueryHandler(context, user);

        var result = await handler.Handle(new GetMyUserProfileQuery(), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(userId, result.Data!.UserId);
        Assert.Equal("Self Profile", result.Data!.DisplayName);
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
