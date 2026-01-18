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
/// Handles retrieval of the current user's roles with optional filtering and sorting.
/// </summary>
public class GetMyRolesQueryHandler : IRequestHandler<GetMyRolesQuery, BaseResponse<PaginatedEnumerable<RoleDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMyRolesQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="user">The current user accessor.</param>
    public GetMyRolesQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    /// <summary>
    /// Handles the roles query request for the current user.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated roles response.</returns>
    public async Task<BaseResponse<PaginatedEnumerable<RoleDto>>> Handle(
        GetMyRolesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _user.UserId;
        if (userId is null)
        {
            throw new UnauthorizedAccessException();
        }

        var directRoleIds = _context.UserRoles
            .Where(userRole => userRole.UserId == userId.Value)
            .Select(userRole => userRole.RoleId);

        var groupRoleIds = _context.GroupRoles
            .Where(groupRole => _context.UserGroups.Any(userGroup =>
                userGroup.UserId == userId.Value && userGroup.GroupId == groupRole.GroupId))
            .Select(groupRole => groupRole.RoleId);

        var roleIds = directRoleIds.Union(groupRoleIds).Distinct();

        var query = _context.Roles
            .AsQueryable()
            .Include(role => role.RolePermissions)
            .ThenInclude(rolePermission => rolePermission.Permission)
            .Where(role => roleIds.Contains(role.Id))
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
