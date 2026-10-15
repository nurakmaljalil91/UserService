#nullable enable
using Application.Common.Interfaces;
using Application.PersonalAccessTokens.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.PersonalAccessTokens.Queries;

/// <summary>
/// Handles <see cref="GetMyPersonalAccessTokensQuery"/>.
/// </summary>
public class GetMyPersonalAccessTokensQueryHandler
    : IRequestHandler<GetMyPersonalAccessTokensQuery, BaseResponse<IReadOnlyList<PersonalAccessTokenDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;
    private readonly IClockService _clock;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMyPersonalAccessTokensQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="user">The current authenticated user.</param>
    /// <param name="clock">The clock service.</param>
    public GetMyPersonalAccessTokensQueryHandler(IApplicationDbContext context, IUser user, IClockService clock)
    {
        _context = context;
        _user = user;
        _clock = clock;
    }

    /// <summary>
    /// Lists the tokens.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The current user's tokens, newest first.</returns>
    public async Task<BaseResponse<IReadOnlyList<PersonalAccessTokenDto>>> Handle(
        GetMyPersonalAccessTokensQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _user.UserId ?? throw new UnauthorizedAccessException();
        var now = _clock.Now;

        var tokens = await _context.PersonalAccessTokens
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedDate)
            .ToListAsync(cancellationToken);

        IReadOnlyList<PersonalAccessTokenDto> result = tokens.Select(t => new PersonalAccessTokenDto(t, now)).ToList();
        return BaseResponse<IReadOnlyList<PersonalAccessTokenDto>>.Ok(result, $"Retrieved {result.Count} personal access tokens.");
    }
}
