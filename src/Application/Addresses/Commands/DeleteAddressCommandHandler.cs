#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Mediator;

namespace Application.Addresses.Commands;

/// <summary>
/// Handles the deletion of an address by its identifier.
/// </summary>
public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteAddressCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public DeleteAddressCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the address deletion request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A confirmation message response.</returns>
    public async Task<BaseResponse<string>> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await _context.Addresses.FindAsync(new object[] { request.Id }, cancellationToken);
        if (address == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Address), request.Id.ToString());
        }

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<string>.Ok($"Address with id {request.Id} deleted successfully.");
    }
}
