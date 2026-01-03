using Domain.Entities;
using NodaTime;

namespace Domain.UnitTests.Entities;

/// <summary>
/// Contains unit tests for the <see cref="UserProfile"/> entity.
/// </summary>
public class UserProfileTests
{
    /// <summary>
    /// Verifies that a new <see cref="UserProfile"/> has its default property values initialized as expected.
    /// </summary>
    [Fact]
    public void Defaults_AreInitialized()
    {
        var profile = new UserProfile();

        Assert.Equal(Guid.Empty, profile.UserId);
        Assert.Null(profile.DisplayName);
        Assert.Null(profile.FirstName);
        Assert.Null(profile.LastName);
        Assert.Null(profile.IdentityCardNumber);
        Assert.Null(profile.PassportNumber);
        Assert.Null(profile.DateOfBirth);
        Assert.Null(profile.BirthPlace);
        Assert.Null(profile.ShoeSize);
        Assert.Null(profile.ClothingSize);
        Assert.Null(profile.WaistSize);
        Assert.Null(profile.Bio);
        Assert.Null(profile.ImageUrl);
        Assert.Null(profile.Tag);
        Assert.Null(profile.BloodType);
        Assert.Null(profile.User);
    }

    /// <summary>
    /// Verifies that the properties of <see cref="UserProfile"/> can be set and retrieved as expected.
    /// </summary>
    [Fact]
    public void Properties_CanBeSet()
    {
        var user = new User { Username = "user" };
        var userId = Guid.NewGuid();

        var dateOfBirth = new LocalDate(1990, 5, 12);

        var profile = new UserProfile
        {
            UserId = userId,
            DisplayName = "Display Name",
            FirstName = "First",
            LastName = "Last",
            IdentityCardNumber = "ID123",
            PassportNumber = "P1234567",
            DateOfBirth = dateOfBirth,
            BirthPlace = "Birth Place",
            ShoeSize = "42",
            ClothingSize = "M",
            WaistSize = "32",
            Bio = "Bio",
            ImageUrl = "https://example.test/avatar.png",
            Tag = "tag",
            BloodType = "O+",
            User = user
        };

        Assert.Equal(userId, profile.UserId);
        Assert.Equal("Display Name", profile.DisplayName);
        Assert.Equal("First", profile.FirstName);
        Assert.Equal("Last", profile.LastName);
        Assert.Equal("ID123", profile.IdentityCardNumber);
        Assert.Equal("P1234567", profile.PassportNumber);
        Assert.Equal(dateOfBirth, profile.DateOfBirth);
        Assert.Equal("Birth Place", profile.BirthPlace);
        Assert.Equal("42", profile.ShoeSize);
        Assert.Equal("M", profile.ClothingSize);
        Assert.Equal("32", profile.WaistSize);
        Assert.Equal("Bio", profile.Bio);
        Assert.Equal("https://example.test/avatar.png", profile.ImageUrl);
        Assert.Equal("tag", profile.Tag);
        Assert.Equal("O+", profile.BloodType);
        Assert.Same(user, profile.User);
    }
}
