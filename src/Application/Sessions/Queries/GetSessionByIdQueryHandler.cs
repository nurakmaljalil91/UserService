#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Sessions.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Sessions.Queries;

/// <summary>
/// Handles retrieval of a session by identifier.
/// </summary>
public class GetSessionByIdQueryHandler : IRequestHandler<GetSessionByIdQuery, BaseResponse<SessionDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSessionByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetSessionByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the session lookup request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The session response.</returns>
    public async Task<BaseResponse<SessionDto>> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
    {
        var session = await _context.Sessions
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (session == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Session), request.Id.ToString());
        }

        return BaseResponse<SessionDto>.Ok(new SessionDto(session), "Session retrieved.");
    }
}
