#nullable enable
using Application.Sessions.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.UnitTests.Sessions;

/// <summary>
/// Unit tests for <see cref="GetSessionsQueryHandler"/>.
/// </summary>
public class GetSessionsQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler returns a paginated list of sessions.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsPaginatedSessions()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        context.Sessions.AddRange(
            new Session
            {
                UserId = user.Id,
                RefreshToken = "refresh-token-1",
                ExpiresAt = Instant.FromUtc(2026, 1, 1, 0, 0)
            },
            new Session
            {
                UserId = user.Id,
                RefreshToken = "refresh-token-2",
                ExpiresAt = Instant.FromUtc(2026, 1, 2, 0, 0)
            });
        await context.SaveChangesAsync();

        var handler = new GetSessionsQueryHandler(context);

        var result = await handler.Handle(new GetSessionsQuery
        {
            Page = 1,
            Total = 10
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.TotalCount);
        Assert.Equal(2, await context.Sessions.CountAsync());
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
