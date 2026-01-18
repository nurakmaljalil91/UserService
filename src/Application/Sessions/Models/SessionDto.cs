#nullable enable
using System;
using Domain.Entities;
using NodaTime.Text;

namespace Application.Sessions.Models;

/// <summary>
/// Represents a session summary for API responses.
/// </summary>
public sealed record SessionDto
{
    private const string InstantPatternText = "yyyy-MM-dd'T'HH:mm:ss'Z'";

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionDto"/> class.
    /// </summary>
    public SessionDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionDto"/> class from a <see cref="Session"/> entity.
    /// </summary>
    /// <param name="session">The <see cref="Session"/> entity to map from.</param>
    public SessionDto(Session session)
    {
        var instantPattern = InstantPattern.CreateWithInvariantCulture(InstantPatternText);
        Id = session.Id;
        UserId = session.UserId;
        RefreshToken = session.RefreshToken;
        ExpiresAt = instantPattern.Format(session.ExpiresAt);
        RevokedAt = session.RevokedAt.HasValue ? instantPattern.Format(session.RevokedAt.Value) : null;
        IpAddress = session.IpAddress;
        UserAgent = session.UserAgent;
        DeviceName = session.DeviceName;
        IsRevoked = session.IsRevoked;
    }

    /// <summary>
    /// Gets the session identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Gets the refresh token for the session.
    /// </summary>
    public string? RefreshToken { get; init; }

    /// <summary>
    /// Gets the expiration date for the session in UTC.
    /// </summary>
    public string? ExpiresAt { get; init; }

    /// <summary>
    /// Gets the time the session was revoked in UTC.
    /// </summary>
    public string? RevokedAt { get; init; }

    /// <summary>
    /// Gets the IP address associated with the session.
    /// </summary>
    public string? IpAddress { get; init; }

    /// <summary>
    /// Gets the user agent associated with the session.
    /// </summary>
    public string? UserAgent { get; init; }

    /// <summary>
    /// Gets the device name associated with the session.
    /// </summary>
    public string? DeviceName { get; init; }

    /// <summary>
    /// Gets whether the session is revoked.
    /// </summary>
    public bool IsRevoked { get; init; }
}
