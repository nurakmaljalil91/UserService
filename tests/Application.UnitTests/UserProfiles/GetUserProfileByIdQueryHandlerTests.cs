#nullable enable
using Application.Common.Exceptions;
using Application.UserProfiles.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;

namespace Application.UnitTests.UserProfiles;

/// <summary>
/// Unit tests for <see cref="GetUserProfileByIdQueryHandler"/>.
/// </summary>
public class GetUserProfileByIdQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the user profile does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenProfileMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new GetUserProfileByIdQueryHandler(context);

        var query = new GetUserProfileByIdQuery
        {
            Id = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(query, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns the user profile when it exists.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsUserProfile_WhenExists()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        var profile = new UserProfile { User = user, DisplayName = "Profile" };
        context.Users.Add(user);
        context.UserProfiles.Add(profile);
        await context.SaveChangesAsync();

        var handler = new GetUserProfileByIdQueryHandler(context);

        var result = await handler.Handle(new GetUserProfileByIdQuery
        {
            Id = profile.Id
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(profile.Id, result.Data!.Id);
        Assert.Equal(user.Id, result.Data!.UserId);
        Assert.Equal("Profile", result.Data!.DisplayName);
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
}
