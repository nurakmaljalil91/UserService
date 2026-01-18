#nullable enable
using Application.Consents.Models;
using Domain.Common;
using Mediator;

namespace Application.Consents.Commands;

/// <summary>
/// Command to create a new consent.
/// </summary>
public class CreateConsentCommand : IRequest<BaseResponse<ConsentDto>>
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
    /// Gets or sets when the consent was granted or revoked in UTC (yyyy-MM-ddTHH:mm:ssZ).
    /// </summary>
    public string? GrantedAt { get; set; }

    /// <summary>
    /// Gets or sets the consent version.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets the source of the consent.
    /// </summary>
    public string? Source { get; set; }
}
