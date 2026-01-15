#nullable enable
using Domain.Common;

namespace Domain.ValueObjects;

/// <summary>
/// Represents a provider-specific subject identifier for an external account.
/// </summary>
public sealed class ExternalSubjectId : ValueObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalSubjectId"/> class.
    /// </summary>
    /// <param name="value">The external subject identifier value.</param>
    /// <exception cref="ArgumentException">Thrown when the value is null or whitespace.</exception>
    public ExternalSubjectId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Subject identifier value is required.", nameof(value));
        }

        Value = value.Trim();
    }

    /// <summary>
    /// Gets the normalized external subject identifier value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a new <see cref="ExternalSubjectId"/> from the specified value.
    /// </summary>
    /// <param name="value">The external subject identifier value.</param>
    /// <returns>The created <see cref="ExternalSubjectId"/> instance.</returns>
    public static ExternalSubjectId From(string value)
    {
        return new ExternalSubjectId(value);
    }

    /// <inheritdoc />
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
