#nullable enable
using Application.Sessions.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using NodaTime;
using System.Linq;

namespace Application.UnitTests.Sessions;

/// <summary>
/// Unit tests for <see cref="GetMySessionsQueryHandler"/>.
/// </summary>
public class GetMySessionsQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the user identifier is missing.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserIdMissing_ThrowsUnauthorizedAccessException()
    {
        await using var context = TestDbContextFactory.Create();
        var user = new TestUser();
        var handler = new GetMySessionsQueryHandler(context, user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new GetMySessionsQuery(), CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns only the current user's sessions.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsCurrentUserSessions()
    {
        await using var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var user = new TestUser { UserId = userId };

        context.Users.AddRange(CreateUser(userId), CreateUser(otherUserId));
        context.Sessions.AddRange(
            new Session
            {
                UserId = userId,
                RefreshToken = "token-1",
                ExpiresAt = Instant.FromUtc(2024, 1, 1, 0, 0)
            },
            new Session
            {
                UserId = otherUserId,
                RefreshToken = "token-2",
                ExpiresAt = Instant.FromUtc(2024, 1, 2, 0, 0)
            });
        await context.SaveChangesAsync();

        var handler = new GetMySessionsQueryHandler(context, user);

        var result = await handler.Handle(new GetMySessionsQuery { Page = 1, Total = 10 }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data!.Items!);
        Assert.Equal(userId, result.Data.Items!.First().UserId);
    }

    /// <summary>
    /// Creates a valid user entity for test scenarios.
    /// </summary>
    /// <param name="userId">The identifier to assign to the user.</param>
    /// <returns>A configured <see cref="User"/> entity.</returns>
    private static User CreateUser(Guid userId)
    {
        var unique = Guid.NewGuid().ToString("N");
        return new User
        {
            Id = userId,
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
