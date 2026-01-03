using Domain.Entities;

namespace Domain.UnitTests.Entities;

/// <summary>
/// Contains unit tests for the <see cref="Address"/> entity.
/// </summary>
public class AddressTests
{
    /// <summary>
    /// Verifies that a new <see cref="Address"/> has its default property values initialized as expected.
    /// </summary>
    [Fact]
    public void Defaults_AreInitialized()
    {
        var address = new Address();

        Assert.Equal(Guid.Empty, address.UserId);
        Assert.Null(address.Label);
        Assert.Null(address.Type);
        Assert.Null(address.Line1);
        Assert.Null(address.Line2);
        Assert.Null(address.City);
        Assert.Null(address.State);
        Assert.Null(address.PostalCode);
        Assert.Null(address.Country);
        Assert.False(address.IsDefault);
        Assert.Null(address.User);
    }

    /// <summary>
    /// Verifies that the properties of <see cref="Address"/> can be set and retrieved as expected.
    /// </summary>
    [Fact]
    public void Properties_CanBeSet()
    {
        var user = new User { Username = "user" };
        var userId = Guid.NewGuid();

        var address = new Address
        {
            UserId = userId,
            Label = "Home",
            Type = "Home",
            Line1 = "123 Main St",
            Line2 = "Apt 4",
            City = "City",
            State = "State",
            PostalCode = "12345",
            Country = "Country",
            IsDefault = true,
            User = user
        };

        Assert.Equal(userId, address.UserId);
        Assert.Equal("Home", address.Label);
        Assert.Equal("Home", address.Type);
        Assert.Equal("123 Main St", address.Line1);
        Assert.Equal("Apt 4", address.Line2);
        Assert.Equal("City", address.City);
        Assert.Equal("State", address.State);
        Assert.Equal("12345", address.PostalCode);
        Assert.Equal("Country", address.Country);
        Assert.True(address.IsDefault);
        Assert.Same(user, address.User);
    }
}
