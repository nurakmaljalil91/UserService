using Domain.Entities;

namespace Domain.UnitTests.Entities;

/// <summary>
/// Contains unit tests for the <see cref="UserPreference"/> entity.
/// </summary>
public class UserPreferenceTests
{
    /// <summary>
    /// Verifies that a new <see cref="UserPreference"/> has its default property values initialized as expected.
    /// </summary>
    [Fact]
    public void Defaults_AreInitialized()
    {
        var preference = new UserPreference();

        Assert.Equal(Guid.Empty, preference.UserId);
        Assert.Null(preference.Key);
        Assert.Null(preference.Value);
        Assert.Null(preference.User);
    }

    /// <summary>
    /// Verifies that the properties of <see cref="UserPreference"/> can be set and retrieved as expected.
    /// </summary>
    [Fact]
    public void Properties_CanBeSet()
    {
        var user = new User { Username = "user" };
        var userId = Guid.NewGuid();

        var preference = new UserPreference
        {
            UserId = userId,
            Key = "locale",
            Value = "en-US",
            User = user
        };

        Assert.Equal(userId, preference.UserId);
        Assert.Equal("locale", preference.Key);
        Assert.Equal("en-US", preference.Value);
        Assert.Same(user, preference.User);
    }
}
