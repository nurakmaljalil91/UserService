#nullable enable
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Users.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries;

/// <summary>
/// Represents a paginated request to retrieve users with optional filtering and sorting.
/// </summary>
public class GetUsersQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<UserDto>>>;

/// <summary>
/// Handles retrieval of paginated users with optional filtering and sorting.
/// </summary>
public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, BaseResponse<PaginatedEnumerable<UserDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<PaginatedEnumerable<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users
            .AsQueryable()
            .Where(u => !u.IsDeleted)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role!)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserGroups)
            .ThenInclude(ug => ug.Group!)
            .ThenInclude(g => g.GroupRoles)
            .ThenInclude(gr => gr.Role!)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .ApplyFilters(request.Filter)
            .ApplySorting(request.SortBy, request.Descending);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.Total <= 0 ? 10 : request.Total;

        var totalCount = await query.CountAsync(cancellationToken);

        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(user => new UserDto(user))
            .ToListAsync(cancellationToken);

        var paginatedResult = new PaginatedEnumerable<UserDto>(result, totalCount, page, pageSize);

        return BaseResponse<PaginatedEnumerable<UserDto>>.Ok(paginatedResult, $"Successfully retrieved {paginatedResult.Items?.Count() ?? 0} users.");
    }
}
