#nullable enable
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.ContactMethods.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.ContactMethods.Queries;

/// <summary>
/// Handles retrieval of paginated contact methods with optional filtering and sorting.
/// </summary>
public class GetContactMethodsQueryHandler : IRequestHandler<GetContactMethodsQuery, BaseResponse<PaginatedEnumerable<ContactMethodDto>>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetContactMethodsQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetContactMethodsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the contact methods query request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated contact methods response.</returns>
    public async Task<BaseResponse<PaginatedEnumerable<ContactMethodDto>>> Handle(
        GetContactMethodsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.ContactMethods
            .AsQueryable()
            .ApplyFilters(request.Filter)
            .ApplySorting(request.SortBy, request.Descending);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.Total <= 0 ? 10 : request.Total;

        var totalCount = await query.CountAsync(cancellationToken);

        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(contactMethod => new ContactMethodDto(contactMethod))
            .ToListAsync(cancellationToken);

        var paginatedResult = new PaginatedEnumerable<ContactMethodDto>(result, totalCount, page, pageSize);

        return BaseResponse<PaginatedEnumerable<ContactMethodDto>>.Ok(
            paginatedResult,
            $"Successfully retrieved {paginatedResult.Items?.Count() ?? 0} contact methods.");
    }
}
