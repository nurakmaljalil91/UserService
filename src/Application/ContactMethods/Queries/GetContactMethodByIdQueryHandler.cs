#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.ContactMethods.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.ContactMethods.Queries;

/// <summary>
/// Handles retrieval of a contact method by identifier.
/// </summary>
public class GetContactMethodByIdQueryHandler : IRequestHandler<GetContactMethodByIdQuery, BaseResponse<ContactMethodDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetContactMethodByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetContactMethodByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the contact method lookup request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The contact method response.</returns>
    public async Task<BaseResponse<ContactMethodDto>> Handle(GetContactMethodByIdQuery request, CancellationToken cancellationToken)
    {
        var contactMethod = await _context.ContactMethods
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (contactMethod == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.ContactMethod), request.Id.ToString());
        }

        return BaseResponse<ContactMethodDto>.Ok(new ContactMethodDto(contactMethod), "Contact method retrieved.");
    }
}
