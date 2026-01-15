#nullable enable
using NodaTime;
using Domain.ValueObjects;

namespace Domain.Entities;

/// <summary>
/// Represents an external OAuth token set for a user and provider.
/// </summary>
public class ExternalToken : BaseEntity<Guid>
{
    /// <summary>
    /// Gets or sets the user identifier that owns this token.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the external provider for this token.
    /// </summary>
    public ExternalProvider Provider { get; set; } = null!;

    /// <summary>
    /// Gets or sets the protected access token value.
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the protected refresh token value.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the access token expires.
    /// </summary>
    public Instant ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the OAuth scopes granted for this token.
    /// </summary>
    public string? Scopes { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the token data was last updated.
    /// </summary>
    public Instant UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the user that owns this token.
    /// </summary>
    public User? User { get; set; }
}
