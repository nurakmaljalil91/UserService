#nullable enable
using Application.Addresses.Models;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Addresses.Commands;

/// <summary>
/// Handles updating an address.
/// </summary>
public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, BaseResponse<AddressDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateAddressCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UpdateAddressCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the address update request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated address response.</returns>
    public async Task<BaseResponse<AddressDto>> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (address == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Address), request.Id.ToString());
        }

        if (request.Label != null)
        {
            address.Label = request.Label.Trim();
        }

        if (request.Type != null)
        {
            address.Type = request.Type.Trim();
        }

        if (request.Line1 != null)
        {
            var line1 = request.Line1.Trim();
            if (string.IsNullOrWhiteSpace(line1))
            {
                return BaseResponse<AddressDto>.Fail("Address line1 is required.");
            }

            address.Line1 = line1;
        }

        if (request.Line2 != null)
        {
            address.Line2 = request.Line2.Trim();
        }

        if (request.City != null)
        {
            address.City = request.City.Trim();
        }

        if (request.State != null)
        {
            address.State = request.State.Trim();
        }

        if (request.PostalCode != null)
        {
            address.PostalCode = request.PostalCode.Trim();
        }

        if (request.Country != null)
        {
            address.Country = request.Country.Trim();
        }

        if (request.IsDefault.HasValue)
        {
            address.IsDefault = request.IsDefault.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<AddressDto>.Ok(new AddressDto(address), "Address updated.");
    }
}
