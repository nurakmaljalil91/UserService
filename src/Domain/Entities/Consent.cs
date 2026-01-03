#nullable enable
using System;
using NodaTime;

namespace Domain.Entities;

/// <summary>
/// Represents a consent record for a user.
/// </summary>
public class Consent : BaseEntity<Guid>
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the consent type.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets whether the consent is granted.
    /// </summary>
    public bool IsGranted { get; set; }

    /// <summary>
    /// Gets or sets when the consent was granted or revoked.
    /// </summary>
    public Instant GrantedAt { get; set; }

    /// <summary>
    /// Gets or sets the consent version.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets the source of the consent.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets the user that owns the consent.
    /// </summary>
    public User? User { get; set; }
}
