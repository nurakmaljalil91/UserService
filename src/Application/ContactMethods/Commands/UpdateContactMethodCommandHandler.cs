#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.ContactMethods.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.ContactMethods.Commands;

/// <summary>
/// Handles updating a contact method.
/// </summary>
public class UpdateContactMethodCommandHandler : IRequestHandler<UpdateContactMethodCommand, BaseResponse<ContactMethodDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateContactMethodCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UpdateContactMethodCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the contact method update request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated contact method response.</returns>
    public async Task<BaseResponse<ContactMethodDto>> Handle(
        UpdateContactMethodCommand request,
        CancellationToken cancellationToken)
    {
        var contactMethod = await _context.ContactMethods.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (contactMethod == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.ContactMethod), request.Id.ToString());
        }

        var type = contactMethod.Type;
        var value = contactMethod.Value;

        if (request.Type != null)
        {
            var nextType = request.Type.Trim();
            if (string.IsNullOrWhiteSpace(nextType))
            {
                return BaseResponse<ContactMethodDto>.Fail("Contact method type is required.");
            }

            type = nextType;
        }

        if (request.Value != null)
        {
            var nextValue = request.Value.Trim();
            if (string.IsNullOrWhiteSpace(nextValue))
            {
                return BaseResponse<ContactMethodDto>.Fail("Contact value is required.");
            }

            value = nextValue;
        }

        if (request.Type != null || request.Value != null)
        {
            var normalizedValue = value?.ToUpperInvariant();
            var exists = await _context.ContactMethods.AnyAsync(
                c => c.Id != contactMethod.Id
                     && c.UserId == contactMethod.UserId
                     && c.Type == type
                     && c.NormalizedValue == normalizedValue,
                cancellationToken);

            if (exists)
            {
                return BaseResponse<ContactMethodDto>.Fail("Contact method already exists.");
            }

            contactMethod.Type = type;
            contactMethod.Value = value;
            contactMethod.NormalizedValue = normalizedValue;
        }

        if (request.IsVerified.HasValue)
        {
            contactMethod.IsVerified = request.IsVerified.Value;
        }

        if (request.IsPrimary.HasValue)
        {
            contactMethod.IsPrimary = request.IsPrimary.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<ContactMethodDto>.Ok(new ContactMethodDto(contactMethod), "Contact method updated.");
    }
}
