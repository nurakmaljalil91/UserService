#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Consents.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Consents.Queries;

/// <summary>
/// Handles retrieval of a consent by identifier.
/// </summary>
public class GetConsentByIdQueryHandler : IRequestHandler<GetConsentByIdQuery, BaseResponse<ConsentDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetConsentByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetConsentByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the consent lookup request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The consent response.</returns>
    public async Task<BaseResponse<ConsentDto>> Handle(GetConsentByIdQuery request, CancellationToken cancellationToken)
    {
        var consent = await _context.Consents
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (consent == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Consent), request.Id.ToString());
        }

        return BaseResponse<ConsentDto>.Ok(new ConsentDto(consent), "Consent retrieved.");
    }
}
