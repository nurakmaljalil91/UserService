#nullable enable
using Application.Common.Exceptions;
using Application.Sessions.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.UnitTests.Sessions;

/// <summary>
/// Unit tests for <see cref="DeleteSessionCommandHandler"/>.
/// </summary>
public class DeleteSessionCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the session does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenSessionMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new DeleteSessionCommandHandler(context);

        var command = new DeleteSessionCommand
        {
            Id = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler deletes the session when it exists.
    /// </summary>
    [Fact]
    public async Task Handle_DeletesSession_WhenExists()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        var session = new Session
        {
            UserId = user.Id,
            RefreshToken = "refresh-token",
            ExpiresAt = Instant.FromUtc(2026, 1, 1, 0, 0)
        };
        context.Users.Add(user);
        context.Sessions.Add(session);
        await context.SaveChangesAsync();

        var handler = new DeleteSessionCommandHandler(context);

        var result = await handler.Handle(new DeleteSessionCommand
        {
            Id = session.Id
        }, CancellationToken.None);

        Assert.True(result.Success);
        var remaining = await context.Sessions.CountAsync();
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
