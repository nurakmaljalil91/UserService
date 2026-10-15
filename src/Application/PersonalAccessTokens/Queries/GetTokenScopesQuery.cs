#nullable enable
using Domain.Common;
using Domain.ValueObjects;
using Mediator;

namespace Application.PersonalAccessTokens.Queries;

/// <summary>
/// Lists the scopes a personal access token can be granted.
/// </summary>
public class GetTokenScopesQuery : IRequest<BaseResponse<IReadOnlyList<string>>>
{
}

/// <summary>
/// Handles <see cref="GetTokenScopesQuery"/>.
/// </summary>
public class GetTokenScopesQueryHandler : IRequestHandler<GetTokenScopesQuery, BaseResponse<IReadOnlyList<string>>>
{
    /// <summary>
    /// Returns the supported scopes.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The supported scopes.</returns>
    public Task<BaseResponse<IReadOnlyList<string>>> Handle(GetTokenScopesQuery request, CancellationToken cancellationToken)
        => Task.FromResult(BaseResponse<IReadOnlyList<string>>.Ok(TokenScope.All, "Supported scopes."));
}
