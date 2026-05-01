#nullable enable
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Languages.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Languages.Queries;

/// <summary>
/// Handles retrieval of paginated languages with optional filtering.
/// </summary>
public class GetLanguagesQueryHandler : IRequestHandler<GetLanguagesQuery, BaseResponse<PaginatedEnumerable<LanguageDto>>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetLanguagesQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetLanguagesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the languages query request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated languages response.</returns>
    public async Task<BaseResponse<PaginatedEnumerable<LanguageDto>>> Handle(
        GetLanguagesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Languages.AsQueryable();

        if (request.UserId.HasValue)
        {
            query = query.Where(l => l.UserId == request.UserId.Value);
        }

        query = query
            .ApplyFilters(request.Filter)
            .ApplySorting(request.SortBy, request.Descending);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.Total <= 0 ? 10 : request.Total;

        var totalCount = await query.CountAsync(cancellationToken);

        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new LanguageDto(l))
            .ToListAsync(cancellationToken);

        var paginatedResult = new PaginatedEnumerable<LanguageDto>(result, totalCount, page, pageSize);

        return BaseResponse<PaginatedEnumerable<LanguageDto>>.Ok(
            paginatedResult,
            $"Successfully retrieved {paginatedResult.Items?.Count() ?? 0} languages.");
    }
}
