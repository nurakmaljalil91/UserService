#nullable enable
using Application.PersonalAccessTokens.Models;
using Domain.Common;
using Mediator;

namespace Application.PersonalAccessTokens.Commands;

/// <summary>
/// Revokes one of the current user's personal access tokens.
/// </summary>
public class RevokePersonalAccessTokenCommand : IRequest<BaseResponse<PersonalAccessTokenDto>>
{
    /// <summary>Gets or sets the token identifier.</summary>
    public Guid Id { get; set; }
}
