#nullable enable
namespace Application.ExternalLinks.Models;

/// <summary>
/// Represents an OAuth token response for an external provider.
/// </summary>
public sealed record ExternalOAuthToken
{
    /// <summary>
    /// Gets the access token value.
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// Gets the refresh token value, if provided.
    /// </summary>
    public string? RefreshToken { get; init; }

    /// <summary>
    /// Gets the access token expiration duration in seconds.
    /// </summary>
    public int ExpiresInSeconds { get; init; }

    /// <summary>
    /// Gets the space-delimited OAuth scopes.
    /// </summary>
    public string? Scope { get; init; }

    /// <summary>
    /// Gets the token type value.
    /// </summary>
    public string? TokenType { get; init; }
}
