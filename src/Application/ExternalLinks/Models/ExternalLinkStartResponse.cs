#nullable enable
namespace Application.ExternalLinks.Models;

/// <summary>
/// Represents the response for starting an external account link flow.
/// </summary>
public sealed record ExternalLinkStartResponse
{
    /// <summary>
    /// Gets the authorization URL to redirect the user to.
    /// </summary>
    public string AuthorizationUrl { get; init; } = string.Empty;

    /// <summary>
    /// Gets the state value used to correlate the link request.
    /// </summary>
    public string State { get; init; } = string.Empty;

    /// <summary>
    /// Gets the external provider name for the link.
    /// </summary>
    public string Provider { get; init; } = string.Empty;
}
