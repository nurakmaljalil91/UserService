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
/// Handles retrieval of paginated sessions with optional filtering and sorting.
/// </summary>
public class GetSessionsQueryHandler : IRequestHandler<GetSessionsQuery, BaseResponse<PaginatedEnumerable<SessionDto>>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSessionsQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetSessionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the sessions query request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated sessions response.</returns>
    public async Task<BaseResponse<PaginatedEnumerable<SessionDto>>> Handle(
        GetSessionsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Sessions
            .AsQueryable()
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
