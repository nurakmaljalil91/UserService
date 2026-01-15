#nullable enable
namespace Application.ExternalLinks.Models;

/// <summary>
/// Represents a user profile returned by an external OAuth provider.
/// </summary>
public sealed record ExternalOAuthUserProfile
{
    /// <summary>
    /// Gets the provider-specific subject identifier.
    /// </summary>
    public string SubjectId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the email address for the external account, if available.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Gets the display name for the external account, if available.
    /// </summary>
    public string? DisplayName { get; init; }
}
