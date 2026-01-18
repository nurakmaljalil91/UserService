#nullable enable
using Application.Sessions.Models;
using Domain.Common;
using Mediator;

namespace Application.Sessions.Commands;

/// <summary>
/// Command to create a new session.
/// </summary>
public class CreateSessionCommand : IRequest<BaseResponse<SessionDto>>
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
    /// Gets or sets the expiration date for the session in UTC (yyyy-MM-ddTHH:mm:ssZ).
    /// </summary>
    public string? ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the revocation time in UTC (yyyy-MM-ddTHH:mm:ssZ).
    /// </summary>
    public string? RevokedAt { get; set; }

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
}
