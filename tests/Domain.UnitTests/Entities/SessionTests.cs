using Domain.Entities;
using NodaTime;

namespace Domain.UnitTests.Entities;

/// <summary>
/// Contains unit tests for the <see cref="Session"/> entity.
/// </summary>
public class SessionTests
{
    /// <summary>
    /// Verifies that a new <see cref="Session"/> has its default property values initialized as expected.
    /// </summary>
    [Fact]
    public void Defaults_AreInitialized()
    {
        var session = new Session();

        Assert.Equal(Guid.Empty, session.UserId);
        Assert.Null(session.RefreshToken);
        Assert.Equal(default(Instant), session.ExpiresAt);
        Assert.Null(session.RevokedAt);
        Assert.Null(session.IpAddress);
        Assert.Null(session.UserAgent);
        Assert.Null(session.DeviceName);
        Assert.False(session.IsRevoked);
        Assert.Null(session.User);
    }

    /// <summary>
    /// Verifies that the properties of <see cref="Session"/> can be set and retrieved as expected.
    /// </summary>
    [Fact]
    public void Properties_CanBeSet()
    {
        var user = new User { Username = "user" };
        var expiresAt = Instant.FromUtc(2025, 1, 1, 0, 0);
        var revokedAt = Instant.FromUtc(2025, 1, 2, 0, 0);
        var userId = Guid.NewGuid();

        var session = new Session
        {
            UserId = userId,
            RefreshToken = "refresh-token",
            ExpiresAt = expiresAt,
            RevokedAt = revokedAt,
            IpAddress = "127.0.0.1",
            UserAgent = "unit-test",
            DeviceName = "device",
            IsRevoked = true,
            User = user
        };

        Assert.Equal(userId, session.UserId);
        Assert.Equal("refresh-token", session.RefreshToken);
        Assert.Equal(expiresAt, session.ExpiresAt);
        Assert.Equal(revokedAt, session.RevokedAt);
        Assert.Equal("127.0.0.1", session.IpAddress);
        Assert.Equal("unit-test", session.UserAgent);
        Assert.Equal("device", session.DeviceName);
        Assert.True(session.IsRevoked);
        Assert.Same(user, session.User);
    }
}
