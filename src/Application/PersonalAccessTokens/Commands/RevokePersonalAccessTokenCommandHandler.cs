#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.PersonalAccessTokens.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.PersonalAccessTokens.Commands;

/// <summary>
/// Handles <see cref="RevokePersonalAccessTokenCommand"/>. Tokens owned by other users are reported as not found.
/// </summary>
public class RevokePersonalAccessTokenCommandHandler
    : IRequestHandler<RevokePersonalAccessTokenCommand, BaseResponse<PersonalAccessTokenDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;
    private readonly IClockService _clock;

    /// <summary>
    /// Initializes a new instance of the <see cref="RevokePersonalAccessTokenCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="user">The current authenticated user.</param>
    /// <param name="clock">The clock service.</param>
    public RevokePersonalAccessTokenCommandHandler(IApplicationDbContext context, IUser user, IClockService clock)
    {
        _context = context;
        _user = user;
        _clock = clock;
    }

    /// <summary>
    /// Revokes the token.
    /// </summary>
    /// <param name="request">The command.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The revoked token's metadata.</returns>
    public async Task<BaseResponse<PersonalAccessTokenDto>> Handle(
        RevokePersonalAccessTokenCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _user.UserId ?? throw new UnauthorizedAccessException();
        var token = await _context.PersonalAccessTokens
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.PersonalAccessToken), request.Id.ToString());

        token.Revoke(_clock.Now);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<PersonalAccessTokenDto>.Ok(
            new PersonalAccessTokenDto(token, _clock.Now),
            "Personal access token revoked.");
    }
}
