#nullable enable
using System;
using NodaTime;

namespace Domain.Entities;

/// <summary>
/// Represents an authentication session for a user.
/// </summary>
public class Session : BaseEntity<Guid>
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the refresh token for the session.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the expiration date for the session.
    /// </summary>
    public Instant ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the time the session was revoked, if applicable.
    /// </summary>
    public Instant? RevokedAt { get; set; }

    /// <summary>
    /// Gets or sets the IP address associated with the session.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the user agent associated with the session.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Gets or sets the device name associated with the session.
    /// </summary>
    public string? DeviceName { get; set; }

    /// <summary>
    /// Gets or sets whether the session is revoked.
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// Gets or sets the user that owns the session.
    /// </summary>
    public User? User { get; set; }
}
