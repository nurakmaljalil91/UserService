#nullable enable
using Application.UserPreferences.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.UserPreferences;

/// <summary>
/// Unit tests for <see cref="CreateUserPreferenceCommandHandler"/>.
/// </summary>
public class CreateUserPreferenceCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler creates a user preference when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_CreatesUserPreference_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateUserPreferenceCommandHandler(context);

        var result = await handler.Handle(new CreateUserPreferenceCommand
        {
            UserId = user.Id,
            Key = " ui.theme ",
            Value = " dark "
        }, CancellationToken.None);

        Assert.True(result.Success);

        var preference = await context.UserPreferences.SingleAsync();
        Assert.Equal(user.Id, preference.UserId);
        Assert.Equal("ui.theme", preference.Key);
        Assert.Equal("dark", preference.Value);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the user does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserMissing_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new CreateUserPreferenceCommandHandler(context);

        var result = await handler.Handle(new CreateUserPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            Key = "ui.theme"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User does not exist.", result.Message);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the key already exists for the user.
    /// </summary>
    [Fact]
    public async Task Handle_WhenKeyExists_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        context.UserPreferences.Add(new UserPreference { UserId = user.Id, Key = "ui.theme", Value = "dark" });
        await context.SaveChangesAsync();

        var handler = new CreateUserPreferenceCommandHandler(context);

        var result = await handler.Handle(new CreateUserPreferenceCommand
        {
            UserId = user.Id,
            Key = "ui.theme"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User preference key already exists.", result.Message);
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
