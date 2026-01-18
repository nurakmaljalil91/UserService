#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Mediator;

namespace Application.ContactMethods.Commands;

/// <summary>
/// Handles the deletion of a contact method by its identifier.
/// </summary>
public class DeleteContactMethodCommandHandler : IRequestHandler<DeleteContactMethodCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteContactMethodCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public DeleteContactMethodCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the contact method deletion request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A confirmation message response.</returns>
    public async Task<BaseResponse<string>> Handle(DeleteContactMethodCommand request, CancellationToken cancellationToken)
    {
        var contactMethod = await _context.ContactMethods.FindAsync(new object[] { request.Id }, cancellationToken);
        if (contactMethod == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.ContactMethod), request.Id.ToString());
        }

        _context.ContactMethods.Remove(contactMethod);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<string>.Ok($"Contact method with id {request.Id} deleted successfully.");
    }
}
