#nullable enable
namespace Application.ExternalLinks.Models;

/// <summary>
/// Represents a short-lived access token response for an external provider.
/// </summary>
public sealed record ExternalAccessTokenDto
{
    /// <summary>
    /// Gets the access token value.
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// Gets the UTC timestamp when the access token expires.
    /// </summary>
    public DateTime ExpiresAtUtc { get; init; }
}
