#nullable enable
using Application.Addresses.Models;
using Domain.Common;
using Mediator;

namespace Application.Addresses.Queries;

/// <summary>
/// Query to retrieve an address by identifier.
/// </summary>
public class GetAddressByIdQuery : IRequest<BaseResponse<AddressDto>>
{
    /// <summary>
    /// Gets or sets the address identifier.
    /// </summary>
    public Guid Id { get; set; }
}
