#nullable enable
using Application.PersonalAccessTokens.Models;
using Domain.Common;
using Mediator;

namespace Application.PersonalAccessTokens.Commands;

/// <summary>
/// Creates a personal access token for the current user.
/// </summary>
public class CreatePersonalAccessTokenCommand : IRequest<BaseResponse<CreatedPersonalAccessTokenDto>>
{
    /// <summary>Gets or sets the token name.</summary>
    public string? Name { get; set; }

    /// <summary>Gets or sets the requested scopes, for example <c>planner:write</c>.</summary>
    public List<string> Scopes { get; set; } = [];

    /// <summary>Gets or sets when the token expires, or <c>null</c> for no expiry.</summary>
    public DateTimeOffset? ExpiresAt { get; set; }
}
