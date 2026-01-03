using Domain.Entities;
using Infrastructure.Services;

namespace Infrastructure.UnitTests.Services;

/// <summary>
/// Unit tests for <see cref="PasswordHasherService"/>.
/// </summary>
public class PasswordHasherServiceTests
{
    /// <summary>
    /// Ensures hashing a password returns a non-empty hash that differs from the input.
    /// </summary>
    [Fact]
    public void HashPassword_ReturnsNonEmptyHash()
    {
        var service = new PasswordHasherService();
        var user = new User { Username = "alice", Email = "alice@example.com" };

        var hash = service.HashPassword(user, "P@ssw0rd!");

        Assert.False(string.IsNullOrWhiteSpace(hash));
        Assert.NotEqual("P@ssw0rd!", hash);
    }

    /// <summary>
    /// Ensures verifying a correct password returns true.
    /// </summary>
    [Fact]
    public void VerifyHashedPassword_ReturnsTrueForCorrectPassword()
    {
        var service = new PasswordHasherService();
        var user = new User { Username = "alice", Email = "alice@example.com" };

        var hash = service.HashPassword(user, "P@ssw0rd!");

        var result = service.VerifyHashedPassword(user, hash, "P@ssw0rd!");

        Assert.True(result);
    }

    /// <summary>
    /// Ensures verifying an incorrect password returns false.
    /// </summary>
    [Fact]
    public void VerifyHashedPassword_ReturnsFalseForIncorrectPassword()
    {
        var service = new PasswordHasherService();
        var user = new User { Username = "alice", Email = "alice@example.com" };

        var hash = service.HashPassword(user, "P@ssw0rd!");

        var result = service.VerifyHashedPassword(user, hash, "wrong-password");

        Assert.False(result);
    }
}
