#nullable enable
using Application.PersonalAccessTokens.Models;
using Domain.Common;
using Mediator;

namespace Application.PersonalAccessTokens.Queries;

/// <summary>
/// Lists the current user's personal access tokens, newest first.
/// </summary>
public class GetMyPersonalAccessTokensQuery : IRequest<BaseResponse<IReadOnlyList<PersonalAccessTokenDto>>>
{
}
