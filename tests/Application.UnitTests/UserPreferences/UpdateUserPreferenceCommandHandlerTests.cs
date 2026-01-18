#nullable enable
using Application.Common.Exceptions;
using Application.UserPreferences.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.UserPreferences;

/// <summary>
/// Unit tests for <see cref="UpdateUserPreferenceCommandHandler"/>.
/// </summary>
public class UpdateUserPreferenceCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the user preference does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenPreferenceMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new UpdateUserPreferenceCommandHandler(context);

        var command = new UpdateUserPreferenceCommand
        {
            Id = Guid.NewGuid(),
            Key = "ui.theme"
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the key already exists for the user.
    /// </summary>
    [Fact]
    public async Task Handle_WhenKeyExists_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        var existing = new UserPreference { UserId = user.Id, Key = "ui.theme", Value = "dark" };
        var duplicate = new UserPreference { UserId = user.Id, Key = "ui.locale", Value = "en" };
        context.Users.Add(user);
        context.UserPreferences.AddRange(existing, duplicate);
        await context.SaveChangesAsync();

        var handler = new UpdateUserPreferenceCommandHandler(context);

        var result = await handler.Handle(new UpdateUserPreferenceCommand
        {
            Id = existing.Id,
            Key = "ui.locale"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User preference key already exists.", result.Message);
    }

    /// <summary>
    /// Ensures the handler updates the user preference when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_UpdatesUserPreference_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        var preference = new UserPreference { UserId = user.Id, Key = "ui.theme", Value = "dark" };
        context.Users.Add(user);
        context.UserPreferences.Add(preference);
        await context.SaveChangesAsync();

        var handler = new UpdateUserPreferenceCommandHandler(context);

        var result = await handler.Handle(new UpdateUserPreferenceCommand
        {
            Id = preference.Id,
            Key = "ui.locale",
            Value = " en-US "
        }, CancellationToken.None);

        Assert.True(result.Success);

        var updated = await context.UserPreferences.SingleAsync(p => p.Id == preference.Id);
        Assert.Equal("ui.locale", updated.Key);
        Assert.Equal("en-US", updated.Value);
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
