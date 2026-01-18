#nullable enable
using Application.Sessions.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.UnitTests.Sessions;

/// <summary>
/// Unit tests for <see cref="CreateSessionCommandHandler"/>.
/// </summary>
public class CreateSessionCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler creates a session when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_CreatesSession_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateSessionCommandHandler(context);

        var result = await handler.Handle(new CreateSessionCommand
        {
            UserId = user.Id,
            RefreshToken = " refresh-token ",
            ExpiresAt = "2026-01-01T00:00:00Z",
            RevokedAt = "2026-01-02T00:00:00Z",
            IpAddress = " 127.0.0.1 ",
            UserAgent = " TestAgent ",
            DeviceName = " TestDevice "
        }, CancellationToken.None);

        Assert.True(result.Success);

        var session = await context.Sessions.SingleAsync();
        Assert.Equal(user.Id, session.UserId);
        Assert.Equal("refresh-token", session.RefreshToken);
        Assert.Equal(Instant.FromUtc(2026, 1, 1, 0, 0), session.ExpiresAt);
        Assert.Equal(Instant.FromUtc(2026, 1, 2, 0, 0), session.RevokedAt);
        Assert.Equal("127.0.0.1", session.IpAddress);
        Assert.Equal("TestAgent", session.UserAgent);
        Assert.Equal("TestDevice", session.DeviceName);
        Assert.True(session.IsRevoked);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the user does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserMissing_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new CreateSessionCommandHandler(context);

        var result = await handler.Handle(new CreateSessionCommand
        {
            UserId = Guid.NewGuid(),
            RefreshToken = "refresh-token",
            ExpiresAt = "2026-01-01T00:00:00Z"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User does not exist.", result.Message);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the refresh token already exists.
    /// </summary>
    [Fact]
    public async Task Handle_WhenRefreshTokenExists_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        context.Sessions.Add(new Session
        {
            UserId = user.Id,
            RefreshToken = "refresh-token",
            ExpiresAt = Instant.FromUtc(2026, 1, 1, 0, 0)
        });
        await context.SaveChangesAsync();

        var handler = new CreateSessionCommandHandler(context);

        var result = await handler.Handle(new CreateSessionCommand
        {
            UserId = user.Id,
            RefreshToken = "refresh-token",
            ExpiresAt = "2026-01-01T00:00:00Z"
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
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateSessionCommandHandler(context);

        var result = await handler.Handle(new CreateSessionCommand
        {
            UserId = user.Id,
            RefreshToken = "refresh-token",
            ExpiresAt = "2026-01-01"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("ExpiresAt must be in yyyy-MM-dd'T'HH:mm:ss'Z' format.", result.Message);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the revoked date is invalid.
    /// </summary>
    [Fact]
    public async Task Handle_WhenRevokedAtInvalid_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateSessionCommandHandler(context);

        var result = await handler.Handle(new CreateSessionCommand
        {
            UserId = user.Id,
            RefreshToken = "refresh-token",
            ExpiresAt = "2026-01-01T00:00:00Z",
            RevokedAt = "2026-01-01"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("RevokedAt must be in yyyy-MM-dd'T'HH:mm:ss'Z' format.", result.Message);
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
