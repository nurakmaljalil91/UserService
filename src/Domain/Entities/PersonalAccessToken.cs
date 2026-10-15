#nullable enable
using NodaTime;

namespace Domain.Entities;

/// <summary>
/// A long-lived, revocable credential a user issues to scripts and AI clients. Only a hash of the secret is stored.
/// </summary>
public class PersonalAccessToken : BaseEntity<Guid>
{
    /// <summary>
    /// Gets or sets the identifier of the user who owns the token.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user-facing name of the token.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the leading characters of the secret, shown so users can recognize the token.
    /// </summary>
    public string DisplayPrefix { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the one-way hash of the secret.
    /// </summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the granted scopes (see <see cref="ValueObjects.TokenScope"/>).
    /// </summary>
    public List<string> Scopes { get; set; } = [];

    /// <summary>
    /// Gets or sets when the token expires, or <c>null</c> when it never expires.
    /// </summary>
    public Instant? ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets when the token was last exchanged.
    /// </summary>
    public Instant? LastUsedAt { get; set; }

    /// <summary>
    /// Gets or sets when the token was revoked, or <c>null</c> when it is not revoked.
    /// </summary>
    public Instant? RevokedAt { get; set; }

    /// <summary>
    /// Gets or sets the owning user.
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Determines whether the token can still be exchanged.
    /// </summary>
    /// <param name="now">The current instant.</param>
    /// <returns><c>true</c> when the token is neither revoked nor expired.</returns>
    public bool IsActive(Instant now) => RevokedAt is null && (ExpiresAt is null || ExpiresAt > now);

    /// <summary>
    /// Revokes the token. Revoking an already revoked token keeps the original revocation time.
    /// </summary>
    /// <param name="now">The current instant.</param>
    public void Revoke(Instant now) => RevokedAt ??= now;
}
