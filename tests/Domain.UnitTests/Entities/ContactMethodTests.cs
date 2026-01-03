using Domain.Entities;

namespace Domain.UnitTests.Entities;

/// <summary>
/// Contains unit tests for the <see cref="ContactMethod"/> entity.
/// </summary>
public class ContactMethodTests
{
    /// <summary>
    /// Verifies that a new <see cref="ContactMethod"/> has its default property values initialized as expected.
    /// </summary>
    [Fact]
    public void Defaults_AreInitialized()
    {
        var contact = new ContactMethod();

        Assert.Equal(Guid.Empty, contact.UserId);
        Assert.Null(contact.Type);
        Assert.Null(contact.Value);
        Assert.Null(contact.NormalizedValue);
        Assert.False(contact.IsVerified);
        Assert.False(contact.IsPrimary);
        Assert.Null(contact.User);
    }

    /// <summary>
    /// Verifies that the properties of <see cref="ContactMethod"/> can be set and retrieved as expected.
    /// </summary>
    [Fact]
    public void Properties_CanBeSet()
    {
        var user = new User { Username = "user" };
        var userId = Guid.NewGuid();

        var contact = new ContactMethod
        {
            UserId = userId,
            Type = "Email",
            Value = "user@example.test",
            NormalizedValue = "USER@EXAMPLE.TEST",
            IsVerified = true,
            IsPrimary = true,
            User = user
        };

        Assert.Equal(userId, contact.UserId);
        Assert.Equal("Email", contact.Type);
        Assert.Equal("user@example.test", contact.Value);
        Assert.Equal("USER@EXAMPLE.TEST", contact.NormalizedValue);
        Assert.True(contact.IsVerified);
        Assert.True(contact.IsPrimary);
        Assert.Same(user, contact.User);
    }
}
