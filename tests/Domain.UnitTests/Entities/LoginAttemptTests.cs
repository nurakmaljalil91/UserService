using Domain.Entities;
using NodaTime;

namespace Domain.UnitTests.Entities;

/// <summary>
/// Contains unit tests for the <see cref="LoginAttempt"/> entity.
/// </summary>
public class LoginAttemptTests
{
    /// <summary>
    /// Verifies that a new <see cref="LoginAttempt"/> has its default property values initialized as expected.
    /// </summary>
    [Fact]
    public void Defaults_AreInitialized()
    {
        var attempt = new LoginAttempt();

        Assert.Null(attempt.UserId);
        Assert.Null(attempt.Identifier);
        Assert.Null(attempt.IpAddress);
        Assert.Null(attempt.UserAgent);
        Assert.False(attempt.IsSuccessful);
        Assert.Null(attempt.FailureReason);
        Assert.Equal(default(Instant), attempt.AttemptedAt);
        Assert.Null(attempt.User);
    }

    /// <summary>
    /// Verifies that the properties of <see cref="LoginAttempt"/> can be set and retrieved as expected.
    /// </summary>
    [Fact]
    public void Properties_CanBeSet()
    {
        var user = new User { Username = "user" };
        var attemptedAt = Instant.FromUtc(2025, 1, 3, 0, 0);
        var userId = Guid.NewGuid();

        var attempt = new LoginAttempt
        {
            UserId = userId,
            Identifier = "user@example.test",
            IpAddress = "127.0.0.1",
            UserAgent = "unit-test",
            IsSuccessful = true,
            FailureReason = "none",
            AttemptedAt = attemptedAt,
            User = user
        };

        Assert.Equal(userId, attempt.UserId);
        Assert.Equal("user@example.test", attempt.Identifier);
        Assert.Equal("127.0.0.1", attempt.IpAddress);
        Assert.Equal("unit-test", attempt.UserAgent);
        Assert.True(attempt.IsSuccessful);
        Assert.Equal("none", attempt.FailureReason);
        Assert.Equal(attemptedAt, attempt.AttemptedAt);
        Assert.Same(user, attempt.User);
    }
}
