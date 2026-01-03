#nullable enable
using System;
using NodaTime;

namespace Domain.Entities;

/// <summary>
/// Represents a login attempt for auditing and throttling.
/// </summary>
public class LoginAttempt : BaseEntity<Guid>
{
    /// <summary>
    /// Gets or sets the user identifier if the attempt maps to a user.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Gets or sets the identifier used during login (username/email).
    /// </summary>
    public string? Identifier { get; set; }

    /// <summary>
    /// Gets or sets the IP address used for the attempt.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the user agent for the attempt.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Gets or sets whether the attempt was successful.
    /// </summary>
    public bool IsSuccessful { get; set; }

    /// <summary>
    /// Gets or sets the failure reason if the attempt failed.
    /// </summary>
    public string? FailureReason { get; set; }

    /// <summary>
    /// Gets or sets when the attempt occurred.
    /// </summary>
    public Instant AttemptedAt { get; set; }

    /// <summary>
    /// Gets or sets the user that owns the attempt.
    /// </summary>
    public User? User { get; set; }
}
