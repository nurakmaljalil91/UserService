using Domain.Entities;
using NodaTime;

namespace Domain.UnitTests.Entities;

/// <summary>
/// Contains unit tests for the <see cref="Consent"/> entity.
/// </summary>
public class ConsentTests
{
    /// <summary>
    /// Verifies that a new <see cref="Consent"/> has its default property values initialized as expected.
    /// </summary>
    [Fact]
    public void Defaults_AreInitialized()
    {
        var consent = new Consent();

        Assert.Equal(Guid.Empty, consent.UserId);
        Assert.Null(consent.Type);
        Assert.False(consent.IsGranted);
        Assert.Equal(default(Instant), consent.GrantedAt);
        Assert.Null(consent.Version);
        Assert.Null(consent.Source);
        Assert.Null(consent.User);
    }

    /// <summary>
    /// Verifies that the properties of <see cref="Consent"/> can be set and retrieved as expected.
    /// </summary>
    [Fact]
    public void Properties_CanBeSet()
    {
        var user = new User { Username = "user" };
        var grantedAt = Instant.FromUtc(2025, 1, 4, 0, 0);
        var userId = Guid.NewGuid();

        var consent = new Consent
        {
            UserId = userId,
            Type = "Marketing",
            IsGranted = true,
            GrantedAt = grantedAt,
            Version = "v1",
            Source = "web",
            User = user
        };

        Assert.Equal(userId, consent.UserId);
        Assert.Equal("Marketing", consent.Type);
        Assert.True(consent.IsGranted);
        Assert.Equal(grantedAt, consent.GrantedAt);
        Assert.Equal("v1", consent.Version);
        Assert.Equal("web", consent.Source);
        Assert.Same(user, consent.User);
    }
}
