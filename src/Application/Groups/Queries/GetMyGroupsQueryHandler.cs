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
/// Handles retrieval of the current user's groups with optional filtering and sorting.
/// </summary>
public class GetMyGroupsQueryHandler : IRequestHandler<GetMyGroupsQuery, BaseResponse<PaginatedEnumerable<GroupDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMyGroupsQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="user">The current user accessor.</param>
    public GetMyGroupsQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    /// <summary>
    /// Handles the groups query request for the current user.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated groups response.</returns>
    public async Task<BaseResponse<PaginatedEnumerable<GroupDto>>> Handle(
        GetMyGroupsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _user.UserId;
        if (userId is null)
        {
            throw new UnauthorizedAccessException();
        }

        var query = _context.Groups
            .AsQueryable()
            .Include(group => group.GroupRoles)
            .ThenInclude(groupRole => groupRole.Role)
            .Where(group => group.UserGroups.Any(userGroup => userGroup.UserId == userId.Value))
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
