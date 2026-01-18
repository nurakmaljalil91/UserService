#nullable enable
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.UserProfiles.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.UserProfiles.Queries;

/// <summary>
/// Handles retrieval of paginated user profiles with optional filtering and sorting.
/// </summary>
public class GetUserProfilesQueryHandler : IRequestHandler<GetUserProfilesQuery, BaseResponse<PaginatedEnumerable<UserProfileDto>>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserProfilesQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetUserProfilesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the user profiles query request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated user profiles response.</returns>
    public async Task<BaseResponse<PaginatedEnumerable<UserProfileDto>>> Handle(
        GetUserProfilesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.UserProfiles
            .AsQueryable()
            .ApplyFilters(request.Filter)
            .ApplySorting(request.SortBy, request.Descending);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.Total <= 0 ? 10 : request.Total;

        var totalCount = await query.CountAsync(cancellationToken);

        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(profile => new UserProfileDto(profile))
            .ToListAsync(cancellationToken);

        var paginatedResult = new PaginatedEnumerable<UserProfileDto>(result, totalCount, page, pageSize);

        return BaseResponse<PaginatedEnumerable<UserProfileDto>>.Ok(
            paginatedResult,
            $"Successfully retrieved {paginatedResult.Items?.Count() ?? 0} user profiles.");
    }
}
