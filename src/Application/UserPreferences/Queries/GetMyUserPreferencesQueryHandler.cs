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
/// Handles retrieval of the current user's preferences with optional filtering and sorting.
/// </summary>
public class GetMyUserPreferencesQueryHandler : IRequestHandler<GetMyUserPreferencesQuery, BaseResponse<PaginatedEnumerable<UserPreferenceDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMyUserPreferencesQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="user">The current user accessor.</param>
    public GetMyUserPreferencesQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    /// <summary>
    /// Handles the user preferences query request for the current user.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated user preferences response.</returns>
    public async Task<BaseResponse<PaginatedEnumerable<UserPreferenceDto>>> Handle(
        GetMyUserPreferencesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _user.UserId;
        if (userId is null)
        {
            throw new UnauthorizedAccessException();
        }

        var query = _context.UserPreferences
            .AsQueryable()
            .Where(preference => preference.UserId == userId.Value)
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
