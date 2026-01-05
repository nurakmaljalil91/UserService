#nullable enable
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Roles.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.Queries;

/// <summary>
/// Represents a paginated request to retrieve roles with optional filtering and sorting.
/// </summary>
public class GetRolesQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<RoleDto>>>;

/// <summary>
/// Handles retrieval of paginated roles with optional filtering and sorting.
/// </summary>
public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, BaseResponse<PaginatedEnumerable<RoleDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetRolesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<PaginatedEnumerable<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Roles
            .AsQueryable()
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .ApplyFilters(request.Filter)
            .ApplySorting(request.SortBy, request.Descending);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.Total <= 0 ? 10 : request.Total;

        var totalCount = await query.CountAsync(cancellationToken);

        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(role => new RoleDto(role))
            .ToListAsync(cancellationToken);

        var paginatedResult = new PaginatedEnumerable<RoleDto>(result, totalCount, page, pageSize);

        return BaseResponse<PaginatedEnumerable<RoleDto>>.Ok(
            paginatedResult,
            $"Successfully retrieved {paginatedResult.Items?.Count() ?? 0} roles.");
    }
}
