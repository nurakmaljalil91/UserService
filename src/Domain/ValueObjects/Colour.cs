using System;
using System.Collections.Generic;
using System.Text;
using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

/// <summary>
/// Represents a supported color value object with a hexadecimal code.
/// </summary>
/// <param name="code">Hexadecimal color code (e.g. "#FFFFFF"). If null or whitespace, defaults to "#000000".</param>
public class Colour(string code) : ValueObject
{
    /// <summary>
    /// Creates a <see cref="Colour"/> instance from the specified hexadecimal code.
    /// Throws <see cref="UnsupportedColourException"/> if the code is not supported.
    /// </summary>
    /// <param name="code">The hexadecimal color code.</param>
    /// <returns>A <see cref="Colour"/> instance representing the specified color.</returns>
    /// <exception cref="UnsupportedColourException">Thrown when the color code is not supported.</exception>
    public static Colour From(string code)
    {
        var colour = new Colour(code);

        if (!SupportedColours.Contains(colour))
        {
            throw new UnsupportedColourException(code);
        }

        return colour;
    }

    /// <summary>
    /// Gets a <see cref="Colour"/> instance representing white (#FFFFFF).
    /// </summary>
    public static Colour White => new("#FFFFFF");

    /// <summary>
    /// Gets a <see cref="Colour"/> instance representing red (#FF5733).
    /// </summary>
    public static Colour Red => new("#FF5733");

    /// <summary>
    /// Gets a <see cref="Colour"/> instance representing orange (#FFC300).
    /// </summary>
    public static Colour Orange => new("#FFC300");

    /// <summary>
    /// Gets a <see cref="Colour"/> instance representing yellow (#FFFF66).
    /// </summary>
    public static Colour Yellow => new("#FFFF66");

    /// <summary>
    /// Gets a <see cref="Colour"/> instance representing green (#CCFF99).
    /// </summary>
    public static Colour Green => new("#CCFF99");

    /// <summary>
    /// Gets a <see cref="Colour"/> instance representing blue (#6666FF).
    /// </summary>
    public static Colour Blue => new("#6666FF");

    /// <summary>
    /// Gets a <see cref="Colour"/> instance representing purple (#9966CC).
    /// </summary>
    public static Colour Purple => new("#9966CC");

    /// <summary>
    /// Gets a <see cref="Colour"/> instance representing grey (#999999).
    /// </summary>
    public static Colour Grey => new("#999999");

    /// <summary>
    /// Hexadecimal color code for this <see cref="Colour"/>.
    /// Defaults to <c>"#000000"</c> when the constructor input is null or whitespace.
    /// </summary>
    public string Code { get; private set; } = string.IsNullOrWhiteSpace(code) ? "#000000" : code;

    /// <summary>
    /// Implicit conversion to <see cref="string"/> returning the <see cref="Code"/>.
    /// </summary>
    /// <param name="colour">The colour to convert.</param>
    public static implicit operator string(Colour colour)
    {
        return colour.ToString();
    }

    /// <summary>
    /// Explicit conversion from <see cref="string"/> to <see cref="Colour"/>.
    /// Validates the input against supported colours.
    /// </summary>
    /// <param name="code">Hexadecimal colour code to convert.</param>
    public static explicit operator Colour(string code)
    {
        return From(code);
    }

    /// <summary>
    /// Returns the hexadecimal code for this <see cref="Colour"/>.
    /// </summary>
    public override string ToString()
    {
        return Code;
    }

    /// <summary>
    /// Gets the collection of supported <see cref="Colour"/> values.
    /// Used by <see cref="From(string)"/> to validate supported colours.
    /// </summary>
    protected static IEnumerable<Colour> SupportedColours
    {
        get
        {
            yield return White;
            yield return Red;
            yield return Orange;
            yield return Yellow;
            yield return Green;
            yield return Blue;
            yield return Purple;
            yield return Grey;
        }
    }

    /// <summary>
    /// Enumerates the equality components for value object comparison.
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
}
