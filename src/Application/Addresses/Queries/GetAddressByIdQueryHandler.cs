#nullable enable
using Application.Addresses.Models;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Addresses.Queries;

/// <summary>
/// Handles retrieval of an address by identifier.
/// </summary>
public class GetAddressByIdQueryHandler : IRequestHandler<GetAddressByIdQuery, BaseResponse<AddressDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAddressByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetAddressByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the address lookup request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The address response.</returns>
    public async Task<BaseResponse<AddressDto>> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
    {
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (address == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Address), request.Id.ToString());
        }

        return BaseResponse<AddressDto>.Ok(new AddressDto(address), "Address retrieved.");
    }
}
