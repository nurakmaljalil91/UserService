#nullable enable
using Application.Common.Exceptions;
using Application.UserPreferences.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;

namespace Application.UnitTests.UserPreferences;

/// <summary>
/// Unit tests for <see cref="GetUserPreferenceByIdQueryHandler"/>.
/// </summary>
public class GetUserPreferenceByIdQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the user preference does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenPreferenceMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new GetUserPreferenceByIdQueryHandler(context);

        var query = new GetUserPreferenceByIdQuery
        {
            Id = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(query, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns the user preference when it exists.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsUserPreference_WhenExists()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        var preference = new UserPreference { User = user, Key = "ui.theme", Value = "dark" };
        context.Users.Add(user);
        context.UserPreferences.Add(preference);
        await context.SaveChangesAsync();

        var handler = new GetUserPreferenceByIdQueryHandler(context);

        var result = await handler.Handle(new GetUserPreferenceByIdQuery
        {
            Id = preference.Id
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(preference.Id, result.Data!.Id);
        Assert.Equal(user.Id, result.Data!.UserId);
        Assert.Equal("ui.theme", result.Data!.Key);
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
