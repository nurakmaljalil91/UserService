#nullable enable
using Application.Common.Exceptions;
using Application.UserPreferences.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.UserPreferences;

/// <summary>
/// Unit tests for <see cref="DeleteUserPreferenceCommandHandler"/>.
/// </summary>
public class DeleteUserPreferenceCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the user preference does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenPreferenceMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new DeleteUserPreferenceCommandHandler(context);

        var command = new DeleteUserPreferenceCommand
        {
            Id = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler deletes the user preference when it exists.
    /// </summary>
    [Fact]
    public async Task Handle_DeletesUserPreference_WhenExists()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        var preference = new UserPreference { UserId = user.Id, Key = "ui.theme", Value = "dark" };
        context.Users.Add(user);
        context.UserPreferences.Add(preference);
        await context.SaveChangesAsync();

        var handler = new DeleteUserPreferenceCommandHandler(context);

        var result = await handler.Handle(new DeleteUserPreferenceCommand
        {
            Id = preference.Id
        }, CancellationToken.None);

        Assert.True(result.Success);
        var remaining = await context.UserPreferences.CountAsync();
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
