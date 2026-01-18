#nullable enable
using Application.UserPreferences.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.UserPreferences;

/// <summary>
/// Unit tests for <see cref="GetUserPreferencesQueryHandler"/>.
/// </summary>
public class GetUserPreferencesQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler returns a paginated list of user preferences.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsPaginatedUserPreferences()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        context.UserPreferences.AddRange(
            new UserPreference { UserId = user.Id, Key = "ui.theme", Value = "dark" },
            new UserPreference { UserId = user.Id, Key = "ui.locale", Value = "en" });
        await context.SaveChangesAsync();

        var handler = new GetUserPreferencesQueryHandler(context);

        var result = await handler.Handle(new GetUserPreferencesQuery
        {
            Page = 1,
            Total = 10
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.TotalCount);
        Assert.Equal(2, await context.UserPreferences.CountAsync());
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
