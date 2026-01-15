#nullable enable
using Domain.ValueObjects;

namespace Application.ExternalLinks.Models;

/// <summary>
/// Represents the result of validating an external link state value.
/// </summary>
public sealed record ExternalLinkStateValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the state is valid.
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Gets the user identifier extracted from the state, if valid.
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Gets the external provider extracted from the state, if valid.
    /// </summary>
    public ExternalProvider? Provider { get; init; }

    /// <summary>
    /// Gets the failure reason, if invalid.
    /// </summary>
    public string? Error { get; init; }
}
