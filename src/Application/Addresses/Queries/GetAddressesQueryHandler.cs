#nullable enable
using Application.Addresses.Models;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Addresses.Queries;

/// <summary>
/// Handles retrieval of paginated addresses with optional filtering and sorting.
/// </summary>
public class GetAddressesQueryHandler : IRequestHandler<GetAddressesQuery, BaseResponse<PaginatedEnumerable<AddressDto>>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAddressesQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetAddressesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the addresses query request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated addresses response.</returns>
    public async Task<BaseResponse<PaginatedEnumerable<AddressDto>>> Handle(
        GetAddressesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Addresses
            .AsQueryable()
            .ApplyFilters(request.Filter)
            .ApplySorting(request.SortBy, request.Descending);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.Total <= 0 ? 10 : request.Total;

        var totalCount = await query.CountAsync(cancellationToken);

        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(address => new AddressDto(address))
            .ToListAsync(cancellationToken);

        var paginatedResult = new PaginatedEnumerable<AddressDto>(result, totalCount, page, pageSize);

        return BaseResponse<PaginatedEnumerable<AddressDto>>.Ok(
            paginatedResult,
            $"Successfully retrieved {paginatedResult.Items?.Count() ?? 0} addresses.");
    }
}
