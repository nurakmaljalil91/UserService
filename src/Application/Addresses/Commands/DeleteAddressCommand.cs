#nullable enable
using Domain.Common;
using Mediator;

namespace Application.Addresses.Commands;

/// <summary>
/// Command to delete an address by its identifier.
/// </summary>
public class DeleteAddressCommand : IRequest<BaseResponse<string>>
{
    /// <summary>
    /// Gets or sets the address identifier.
    /// </summary>
    public Guid Id { get; set; }
}
