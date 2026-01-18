#nullable enable
using Application.Common.Exceptions;
using Application.UserProfiles.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.UserProfiles;

/// <summary>
/// Unit tests for <see cref="DeleteUserProfileCommandHandler"/>.
/// </summary>
public class DeleteUserProfileCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the user profile does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenProfileMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new DeleteUserProfileCommandHandler(context);

        var command = new DeleteUserProfileCommand
        {
            Id = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler deletes the user profile when it exists.
    /// </summary>
    [Fact]
    public async Task Handle_DeletesUserProfile_WhenExists()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        var profile = new UserProfile { UserId = user.Id };
        context.Users.Add(user);
        context.UserProfiles.Add(profile);
        await context.SaveChangesAsync();

        var handler = new DeleteUserProfileCommandHandler(context);

        var result = await handler.Handle(new DeleteUserProfileCommand
        {
            Id = profile.Id
        }, CancellationToken.None);

        Assert.True(result.Success);
        var remaining = await context.UserProfiles.CountAsync();
        Assert.Equal(0, remaining);
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
