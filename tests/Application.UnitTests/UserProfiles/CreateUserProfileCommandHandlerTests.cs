#nullable enable
using Application.UserProfiles.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.UserProfiles;

/// <summary>
/// Unit tests for <see cref="CreateUserProfileCommandHandler"/>.
/// </summary>
public class CreateUserProfileCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler creates a user profile when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_CreatesUserProfile_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateUserProfileCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new CreateUserProfileCommand
        {
            UserId = user.Id,
            DisplayName = " Display ",
            FirstName = " First ",
            LastName = " Last ",
            DateOfBirth = "1990-01-02",
            Bio = " Bio "
        }, CancellationToken.None);

        Assert.True(result.Success);

        var profile = await context.UserProfiles.SingleAsync();
        Assert.Equal(user.Id, profile.UserId);
        Assert.Equal("Display", profile.DisplayName);
        Assert.Equal("First", profile.FirstName);
        Assert.Equal("Last", profile.LastName);
        Assert.Equal("1990-01-02", profile.DateOfBirth?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture));
        Assert.Equal("Bio", profile.Bio);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the user does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserMissing_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new CreateUserProfileCommandHandler(context, new TestClockService());
        var result = await handler.Handle(new CreateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            DisplayName = "Display"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User does not exist.", result.Message);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when a profile already exists.
    /// </summary>
    [Fact]
    public async Task Handle_WhenProfileExists_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        // Ensure non-empty Id
        user.Id = Guid.NewGuid();
        context.Users.Add(user);
        await context.SaveChangesAsync();
        // Create profile after user.Id has been generated and with non-empty Id
        var profile = new UserProfile { Id = Guid.NewGuid(), UserId = user.Id };
        context.UserProfiles.Add(profile);
        await context.SaveChangesAsync();

        var handler = new CreateUserProfileCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new CreateUserProfileCommand
        {
            UserId = user.Id,
            DisplayName = "Display"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User profile already exists.", result.Message);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the date of birth is invalid.
    /// </summary>
    [Fact]
    public async Task Handle_WhenDateOfBirthInvalid_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateUserProfileCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new CreateUserProfileCommand
        {
            UserId = user.Id,
            DateOfBirth = "01-02-1990"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Date of birth must be in yyyy-MM-dd format.", result.Message);
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
