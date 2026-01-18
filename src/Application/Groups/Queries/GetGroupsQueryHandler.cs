#nullable enable
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Groups.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Groups.Queries;

/// <summary>
/// Handles retrieval of paginated groups with optional filtering and sorting.
/// </summary>
public class GetGroupsQueryHandler : IRequestHandler<GetGroupsQuery, BaseResponse<PaginatedEnumerable<GroupDto>>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetGroupsQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetGroupsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the groups query request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated groups response.</returns>
    public async Task<BaseResponse<PaginatedEnumerable<GroupDto>>> Handle(
        GetGroupsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Groups
            .AsQueryable()
            .Include(g => g.GroupRoles)
            .ThenInclude(gr => gr.Role)
            .ApplyFilters(request.Filter)
            .ApplySorting(request.SortBy, request.Descending);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.Total <= 0 ? 10 : request.Total;

        var totalCount = await query.CountAsync(cancellationToken);

        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(group => new GroupDto(group))
            .ToListAsync(cancellationToken);

        var paginatedResult = new PaginatedEnumerable<GroupDto>(result, totalCount, page, pageSize);

        return BaseResponse<PaginatedEnumerable<GroupDto>>.Ok(
            paginatedResult,
            $"Successfully retrieved {paginatedResult.Items?.Count() ?? 0} groups.");
    }
}
