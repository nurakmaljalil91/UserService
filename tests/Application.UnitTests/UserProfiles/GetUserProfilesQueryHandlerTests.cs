#nullable enable
using Application.UserProfiles.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.UserProfiles;

/// <summary>
/// Unit tests for <see cref="GetUserProfilesQueryHandler"/>.
/// </summary>
public class GetUserProfilesQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler returns a paginated list of user profiles.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsPaginatedUserProfiles()
    {
        await using var context = TestDbContextFactory.Create();
        var userOne = CreateUser();
        var userTwo = CreateUser();
        context.Users.AddRange(userOne, userTwo);
        context.UserProfiles.AddRange(
            new UserProfile { UserId = userOne.Id, DisplayName = "One" },
            new UserProfile { UserId = userTwo.Id, DisplayName = "Two" });
        await context.SaveChangesAsync();

        var handler = new GetUserProfilesQueryHandler(context);

        var result = await handler.Handle(new GetUserProfilesQuery
        {
            Page = 1,
            Total = 10
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.TotalCount);
        Assert.Equal(2, await context.UserProfiles.CountAsync());
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
