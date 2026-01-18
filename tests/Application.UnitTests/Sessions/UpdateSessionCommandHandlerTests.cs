#nullable enable
using Application.Common.Exceptions;
using Application.Sessions.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.UnitTests.Sessions;

/// <summary>
/// Unit tests for <see cref="UpdateSessionCommandHandler"/>.
/// </summary>
public class UpdateSessionCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the session does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenSessionMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new UpdateSessionCommandHandler(context);

        var command = new UpdateSessionCommand
        {
            Id = Guid.NewGuid(),
            RefreshToken = "refresh-token"
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the refresh token already exists.
    /// </summary>
    [Fact]
    public async Task Handle_WhenRefreshTokenExists_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        var existing = new Session
        {
            UserId = user.Id,
            RefreshToken = "refresh-token",
            ExpiresAt = Instant.FromUtc(2026, 1, 1, 0, 0)
        };
        var duplicate = new Session
        {
            UserId = user.Id,
            RefreshToken = "refresh-token-2",
            ExpiresAt = Instant.FromUtc(2026, 1, 2, 0, 0)
        };
        context.Users.Add(user);
        context.Sessions.AddRange(existing, duplicate);
        await context.SaveChangesAsync();

        var handler = new UpdateSessionCommandHandler(context);

        var result = await handler.Handle(new UpdateSessionCommand
        {
            Id = existing.Id,
            RefreshToken = "refresh-token-2"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Refresh token already exists.", result.Message);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the expiration date is invalid.
    /// </summary>
    [Fact]
    public async Task Handle_WhenExpiresAtInvalid_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var session = CreateSession(context);
        var handler = new UpdateSessionCommandHandler(context);

        var result = await handler.Handle(new UpdateSessionCommand
        {
            Id = session.Id,
            ExpiresAt = "2026-01-01"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("ExpiresAt must be in yyyy-MM-dd'T'HH:mm:ss'Z' format.", result.Message);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the revocation date is invalid.
    /// </summary>
    [Fact]
    public async Task Handle_WhenRevokedAtInvalid_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var session = CreateSession(context);
        var handler = new UpdateSessionCommandHandler(context);

        var result = await handler.Handle(new UpdateSessionCommand
        {
            Id = session.Id,
            RevokedAt = "2026-01-01"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("RevokedAt must be in yyyy-MM-dd'T'HH:mm:ss'Z' format.", result.Message);
    }

    /// <summary>
    /// Ensures the handler updates the session when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_UpdatesSession_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var session = CreateSession(context);
        var handler = new UpdateSessionCommandHandler(context);

        var result = await handler.Handle(new UpdateSessionCommand
        {
            Id = session.Id,
            RefreshToken = "refresh-token-updated",
            ExpiresAt = "2026-02-01T00:00:00Z",
            RevokedAt = "2026-02-02T00:00:00Z",
            IpAddress = "127.0.0.2",
            UserAgent = "Agent",
            DeviceName = "Device",
            IsRevoked = true
        }, CancellationToken.None);

        Assert.True(result.Success);

        var updated = await context.Sessions.SingleAsync(s => s.Id == session.Id);
        Assert.Equal("refresh-token-updated", updated.RefreshToken);
        Assert.Equal(Instant.FromUtc(2026, 2, 1, 0, 0), updated.ExpiresAt);
        Assert.Equal(Instant.FromUtc(2026, 2, 2, 0, 0), updated.RevokedAt);
        Assert.Equal("127.0.0.2", updated.IpAddress);
        Assert.Equal("Agent", updated.UserAgent);
        Assert.Equal("Device", updated.DeviceName);
        Assert.True(updated.IsRevoked);
    }

    /// <summary>
    /// Ensures the handler clears the revocation date when provided with whitespace.
    /// </summary>
    [Fact]
    public async Task Handle_WhenRevokedAtWhitespace_ClearsValue()
    {
        await using var context = TestDbContextFactory.Create();
        var session = CreateSession(context);
        session.RevokedAt = Instant.FromUtc(2026, 1, 2, 0, 0);
        await context.SaveChangesAsync();

        var handler = new UpdateSessionCommandHandler(context);

        var result = await handler.Handle(new UpdateSessionCommand
        {
            Id = session.Id,
            RevokedAt = " "
        }, CancellationToken.None);

        Assert.True(result.Success);

        var updated = await context.Sessions.SingleAsync(s => s.Id == session.Id);
        Assert.Null(updated.RevokedAt);
    }

    /// <summary>
    /// Creates a valid session with a user for test scenarios.
    /// </summary>
    /// <param name="context">The test database context.</param>
    /// <returns>The created <see cref="Session"/> entity.</returns>
    private static Session CreateSession(TestApplicationDbContext context)
    {
        var user = CreateUser();
        var session = new Session
        {
            UserId = user.Id,
            RefreshToken = "refresh-token",
            ExpiresAt = Instant.FromUtc(2026, 1, 1, 0, 0)
        };
        context.Users.Add(user);
        context.Sessions.Add(session);
        context.SaveChanges();
        return session;
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
