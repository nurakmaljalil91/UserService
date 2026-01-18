#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Mediator;

namespace Application.Consents.Commands;

/// <summary>
/// Handles the deletion of a consent by its identifier.
/// </summary>
public class DeleteConsentCommandHandler : IRequestHandler<DeleteConsentCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteConsentCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public DeleteConsentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the consent deletion request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A confirmation message response.</returns>
    public async Task<BaseResponse<string>> Handle(DeleteConsentCommand request, CancellationToken cancellationToken)
    {
        var consent = await _context.Consents.FindAsync(new object[] { request.Id }, cancellationToken);
        if (consent == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Consent), request.Id.ToString());
        }

        _context.Consents.Remove(consent);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<string>.Ok($"Consent with id {request.Id} deleted successfully.");
    }
}
