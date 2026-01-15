#nullable enable
using Domain.Common;

namespace Domain.ValueObjects;

/// <summary>
/// Represents an external authentication provider identifier.
/// </summary>
public sealed class ExternalProvider : ValueObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalProvider"/> class.
    /// </summary>
    /// <param name="value">The provider identifier value.</param>
    /// <exception cref="ArgumentException">Thrown when the value is null or whitespace.</exception>
    public ExternalProvider(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Provider value is required.", nameof(value));
        }

        Value = value.Trim().ToLowerInvariant();
    }

    /// <summary>
    /// Gets the normalized provider identifier value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a new <see cref="ExternalProvider"/> from the specified value.
    /// </summary>
    /// <param name="value">The provider identifier value.</param>
    /// <returns>The created <see cref="ExternalProvider"/> instance.</returns>
    public static ExternalProvider From(string value)
    {
        return new ExternalProvider(value);
    }

    /// <inheritdoc />
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
