#nullable enable
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Permissions.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Permissions.Queries;

/// <summary>
/// Handles retrieval of paginated permissions with optional filtering and sorting.
/// </summary>
public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, BaseResponse<PaginatedEnumerable<PermissionDto>>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPermissionsQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetPermissionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the permissions query request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated permissions response.</returns>
    public async Task<BaseResponse<PaginatedEnumerable<PermissionDto>>> Handle(
        GetPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Permissions
            .AsQueryable()
            .ApplyFilters(request.Filter)
            .ApplySorting(request.SortBy, request.Descending);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.Total <= 0 ? 10 : request.Total;

        var totalCount = await query.CountAsync(cancellationToken);

        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(permission => new PermissionDto(permission))
            .ToListAsync(cancellationToken);

        var paginatedResult = new PaginatedEnumerable<PermissionDto>(result, totalCount, page, pageSize);

        return BaseResponse<PaginatedEnumerable<PermissionDto>>.Ok(
            paginatedResult,
            $"Successfully retrieved {paginatedResult.Items?.Count() ?? 0} permissions.");
    }
}
