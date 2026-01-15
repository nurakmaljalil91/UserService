#nullable enable
namespace Application.ExternalLinks.Models;

/// <summary>
/// Represents a linked external provider for a user.
/// </summary>
public sealed record ExternalLinkDto
{
    /// <summary>
    /// Gets the external provider name.
    /// </summary>
    public string Provider { get; init; } = string.Empty;

    /// <summary>
    /// Gets the provider-specific subject identifier.
    /// </summary>
    public string SubjectId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the external account email address, if available.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Gets the external account display name, if available.
    /// </summary>
    public string? DisplayName { get; init; }

    /// <summary>
    /// Gets the timestamp when the link was created.
    /// </summary>
    public DateTime LinkedAtUtc { get; init; }
}
