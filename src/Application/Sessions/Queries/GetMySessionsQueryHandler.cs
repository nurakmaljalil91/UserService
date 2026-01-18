#nullable enable
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Sessions.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Sessions.Queries;

/// <summary>
/// Handles retrieval of the current user's sessions with optional filtering and sorting.
/// </summary>
public class GetMySessionsQueryHandler : IRequestHandler<GetMySessionsQuery, BaseResponse<PaginatedEnumerable<SessionDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMySessionsQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="user">The current user accessor.</param>
    public GetMySessionsQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    /// <summary>
    /// Handles the sessions query request for the current user.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated sessions response.</returns>
    public async Task<BaseResponse<PaginatedEnumerable<SessionDto>>> Handle(
        GetMySessionsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _user.UserId;
        if (userId is null)
        {
            throw new UnauthorizedAccessException();
        }

        var query = _context.Sessions
            .AsQueryable()
            .Where(session => session.UserId == userId.Value)
            .ApplyFilters(request.Filter)
            .ApplySorting(request.SortBy, request.Descending);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.Total <= 0 ? 10 : request.Total;

        var totalCount = await query.CountAsync(cancellationToken);

        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(session => new SessionDto(session))
            .ToListAsync(cancellationToken);

        var paginatedResult = new PaginatedEnumerable<SessionDto>(result, totalCount, page, pageSize);

        return BaseResponse<PaginatedEnumerable<SessionDto>>.Ok(
            paginatedResult,
            $"Successfully retrieved {paginatedResult.Items?.Count() ?? 0} sessions.");
    }
}
