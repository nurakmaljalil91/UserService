#nullable enable
using Domain.Entities;
using NodaTime;

namespace Domain.UnitTests.Entities;

/// <summary>
/// Unit tests for <see cref="PersonalAccessToken"/>.
/// </summary>
public class PersonalAccessTokenTests
{
    private static readonly Instant Now = Instant.FromUtc(2026, 10, 15, 0, 0);

    /// <summary>
    /// Verifies the active state across expiry and revocation.
    /// </summary>
    [Fact]
    public void IsActive_ReflectsExpiryAndRevocation()
    {
        Assert.True(new PersonalAccessToken().IsActive(Now));
        Assert.True(new PersonalAccessToken { ExpiresAt = Now + Duration.FromDays(1) }.IsActive(Now));
        Assert.False(new PersonalAccessToken { ExpiresAt = Now }.IsActive(Now));
        Assert.False(new PersonalAccessToken { RevokedAt = Now - Duration.FromDays(1) }.IsActive(Now));
    }

    /// <summary>
    /// Verifies that revoking twice keeps the first revocation time.
    /// </summary>
    [Fact]
    public void Revoke_KeepsFirstRevocationTime()
    {
        var token = new PersonalAccessToken();

        token.Revoke(Now);
        token.Revoke(Now + Duration.FromHours(1));

        Assert.Equal(Now, token.RevokedAt);
        Assert.False(token.IsActive(Now));
    }
}
