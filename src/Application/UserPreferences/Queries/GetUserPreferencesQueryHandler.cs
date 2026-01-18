#nullable enable
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.UserPreferences.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.UserPreferences.Queries;

/// <summary>
/// Handles retrieval of paginated user preferences with optional filtering and sorting.
/// </summary>
public class GetUserPreferencesQueryHandler : IRequestHandler<GetUserPreferencesQuery, BaseResponse<PaginatedEnumerable<UserPreferenceDto>>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserPreferencesQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetUserPreferencesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the user preferences query request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated user preferences response.</returns>
    public async Task<BaseResponse<PaginatedEnumerable<UserPreferenceDto>>> Handle(
        GetUserPreferencesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.UserPreferences
            .AsQueryable()
            .ApplyFilters(request.Filter)
            .ApplySorting(request.SortBy, request.Descending);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.Total <= 0 ? 10 : request.Total;

        var totalCount = await query.CountAsync(cancellationToken);

        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(preference => new UserPreferenceDto(preference))
            .ToListAsync(cancellationToken);

        var paginatedResult = new PaginatedEnumerable<UserPreferenceDto>(result, totalCount, page, pageSize);

        return BaseResponse<PaginatedEnumerable<UserPreferenceDto>>.Ok(
            paginatedResult,
            $"Successfully retrieved {paginatedResult.Items?.Count() ?? 0} user preferences.");
    }
}
