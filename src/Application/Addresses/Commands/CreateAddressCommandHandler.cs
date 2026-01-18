#nullable enable
using Application.Addresses.Models;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Addresses.Commands;

/// <summary>
/// Handles creation of a new address.
/// </summary>
public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, BaseResponse<AddressDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateAddressCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public CreateAddressCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the creation of a new address.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created address response.</returns>
    public async Task<BaseResponse<AddressDto>> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists)
        {
            return BaseResponse<AddressDto>.Fail("User does not exist.");
        }

        var address = new Address
        {
            UserId = request.UserId,
            Label = request.Label?.Trim(),
            Type = request.Type?.Trim(),
            Line1 = request.Line1?.Trim(),
            Line2 = request.Line2?.Trim(),
            City = request.City?.Trim(),
            State = request.State?.Trim(),
            PostalCode = request.PostalCode?.Trim(),
            Country = request.Country?.Trim(),
            IsDefault = request.IsDefault
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<AddressDto>.Ok(new AddressDto(address), $"Created address with id {address.Id}");
    }
}
