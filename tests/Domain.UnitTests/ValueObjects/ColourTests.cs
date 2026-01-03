using System;
using System.Linq;
using Domain.Exceptions;
using Domain.ValueObjects;
using Xunit;

namespace Domain.UnitTests.ValueObjects;

/// <summary>
/// Unit tests for the <see cref="Colour"/> value object.
/// </summary>
public class ColourTests
{
    /// <summary>
    /// Ensures that constructing a <see cref="Colour"/> with a null or empty code defaults to black ("#000000").
    /// </summary>
    /// <param name="input">The input color code, which may be null or empty.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WhenCodeIsNullOrWhitespace_DefaultsToBlack(string input)
    {
        // Act
        var colour = new Colour(input);

        // Assert
        Assert.Equal("#000000", colour.Code);
        Assert.Equal("#000000", colour.ToString());
    }

    /// <summary>
    /// Ensures that <see cref="Colour.From(string)"/> returns a <see cref="Colour"/> instance
    /// when provided with a supported color code.
    /// </summary>
    [Fact]
    public void From_WhenCodeIsSupported_ReturnsColour()
    {
        // Act
        var colour = Colour.From("#FFFFFF");

        // Assert
        Assert.NotNull(colour);
        Assert.Equal("#FFFFFF", colour.Code);
        Assert.Equal(Colour.White, colour); // value equality
    }

    /// <summary>
    /// Ensures that <see cref="Colour.From(string)"/> throws an <see cref="UnsupportedColourException"/>
    /// when provided with an unsupported or invalid color code.
    /// </summary>
    /// <param name="code">The input color code to test for exception throwing.</param>
    [Theory]
    [InlineData("#ffffff")] // case-sensitive in your current implementation
    [InlineData("#000000")] // not in supported list
    [InlineData("FFFFFF")]  // invalid format for your supported list
    [InlineData("not-a-colour")]
    public void From_WhenCodeIsNotSupported_ThrowsUnsupportedColourException(string code)
    {
        // Act
        var ex = Assert.Throws<UnsupportedColourException>(() => Colour.From(code));

        // Assert (light-touch: message/props depend on your exception implementation)
        Assert.NotNull(ex);
    }

    /// <summary>
    /// Ensures that all static <see cref="Colour"/> properties have the expected color codes.
    /// </summary>
    [Fact]
    public void StaticColours_HaveExpectedCodes()
    {
        Assert.Equal("#FFFFFF", Colour.White.Code);
        Assert.Equal("#FF5733", Colour.Red.Code);
        Assert.Equal("#FFC300", Colour.Orange.Code);
        Assert.Equal("#FFFF66", Colour.Yellow.Code);
        Assert.Equal("#CCFF99", Colour.Green.Code);
        Assert.Equal("#6666FF", Colour.Blue.Code);
        Assert.Equal("#9966CC", Colour.Purple.Code);
        Assert.Equal("#999999", Colour.Grey.Code);
    }

    /// <summary>
    /// Ensures that <see cref="Colour.ToString()"/> returns the color code.
    /// </summary>
    [Fact]
    public void ToString_ReturnsCode()
    {
        var colour = new Colour("#FFFFFF");
        Assert.Equal("#FFFFFF", colour.ToString());
    }

    /// <summary>
    /// Ensures that implicit conversion of a <see cref="Colour"/> to <see cref="string"/> returns the color code.
    /// </summary>
    [Fact]
    public void ImplicitStringConversion_ReturnsCode()
    {
        var colour = new Colour("#FFFFFF");

        string code = colour;

        Assert.Equal("#FFFFFF", code);
    }

    /// <summary>
    /// Ensures that explicit conversion from <see cref="string"/> to <see cref="Colour"/> validates that the color code is supported.
    /// </summary>
    [Fact]
    public void ExplicitColourConversion_FromString_ValidatesSupported()
    {
        // Act
        var colour = (Colour)"#FFFFFF";

        // Assert
        Assert.Equal(Colour.White, colour);
    }

    /// <summary>
    /// Ensures that explicit conversion from <see cref="string"/> to <see cref="Colour"/> throws an <see cref="UnsupportedColourException"/>
    /// when the color code is not supported.
    /// </summary>
    [Fact]
    public void ExplicitColourConversion_FromString_WhenUnsupported_Throws()
    {
        Assert.Throws<UnsupportedColourException>(() =>
        {
            var _ = (Colour)"#000000";
        });
    }

    /// <summary>
    /// Ensures that two <see cref="Colour"/> instances with the same code are considered equal.
    /// </summary>
    [Fact]
    public void ValueEquality_SameCode_AreEqual()
    {
        var a = new Colour("#FFFFFF");
        var b = new Colour("#FFFFFF");

        Assert.Equal(a, b);
        Assert.True(a.Equals(b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    /// <summary>
    /// Ensures that two <see cref="Colour"/> instances with different codes are not considered equal.
    /// </summary>
    [Fact]
    public void ValueEquality_DifferentCode_AreNotEqual()
    {
        var a = new Colour("#FFFFFF");
        var b = new Colour("#FF5733");

        Assert.NotEqual(a, b);
        Assert.False(a.Equals(b));
    }

    /// <summary>
    /// Ensures that all supported <see cref="Colour"/> instances exposed as static properties
    /// are non-null, have non-empty codes, and that all codes are distinct.
    /// </summary>
    [Fact]
    public void SupportedColours_AllAreDistinctAndNonNull()
    {
        // Because SupportedColours is protected, we can validate indirectly using public static colours
        var supported = new[]
        {
            Colour.White,
            Colour.Red,
            Colour.Orange,
            Colour.Yellow,
            Colour.Green,
            Colour.Blue,
            Colour.Purple,
            Colour.Grey
        };

        Assert.All(supported, c => Assert.False(string.IsNullOrWhiteSpace(c.Code)));
        Assert.Equal(supported.Length, supported.Select(x => x.Code).Distinct().Count());
    }
}
