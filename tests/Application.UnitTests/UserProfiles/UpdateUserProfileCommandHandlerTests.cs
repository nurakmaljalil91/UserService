#nullable enable
using Application.Common.Exceptions;
using Application.UserProfiles.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.UserProfiles;

/// <summary>
/// Unit tests for <see cref="UpdateUserProfileCommandHandler"/>.
/// </summary>
public class UpdateUserProfileCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the user profile does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenProfileMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new UpdateUserProfileCommandHandler(context, new TestClockService());

        var command = new UpdateUserProfileCommand
        {
            Id = Guid.NewGuid(),
            DisplayName = "Display"
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the date of birth is invalid.
    /// </summary>
    [Fact]
    public async Task Handle_WhenDateOfBirthInvalid_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        var profile = new UserProfile { UserId = user.Id };
        context.Users.Add(user);
        context.UserProfiles.Add(profile);
        await context.SaveChangesAsync();

        var handler = new UpdateUserProfileCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new UpdateUserProfileCommand
        {
            Id = profile.Id,
            DateOfBirth = "1990/01/02"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Date of birth must be in yyyy-MM-dd format.", result.Message);
    }

    /// <summary>
    /// Ensures the handler updates the user profile when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_UpdatesUserProfile_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        var profile = new UserProfile { UserId = user.Id, DisplayName = "Old" };
        context.Users.Add(user);
        context.UserProfiles.Add(profile);
        await context.SaveChangesAsync();

        var handler = new UpdateUserProfileCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new UpdateUserProfileCommand
        {
            Id = profile.Id,
            DisplayName = " New ",
            Bio = " Updated ",
            DateOfBirth = "2000-05-06"
        }, CancellationToken.None);

        Assert.True(result.Success);

        var updated = await context.UserProfiles.SingleAsync(p => p.Id == profile.Id);
        Assert.Equal("New", updated.DisplayName);
        Assert.Equal("Updated", updated.Bio);
        Assert.Equal("2000-05-06", updated.DateOfBirth?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Ensures the handler clears the date of birth when provided with whitespace.
    /// </summary>
    [Fact]
    public async Task Handle_WhenDateOfBirthWhitespace_ClearsDate()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        var profile = new UserProfile { UserId = user.Id, DateOfBirth = new NodaTime.LocalDate(1999, 1, 1) };
        context.Users.Add(user);
        context.UserProfiles.Add(profile);
        await context.SaveChangesAsync();

        var handler = new UpdateUserProfileCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new UpdateUserProfileCommand
        {
            Id = profile.Id,
            DateOfBirth = " "
        }, CancellationToken.None);

        Assert.True(result.Success);

        var updated = await context.UserProfiles.SingleAsync(p => p.Id == profile.Id);
        Assert.Null(updated.DateOfBirth);
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
