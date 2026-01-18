#nullable enable
using Application.Common.Interfaces;
using Application.ContactMethods.Models;
using Domain.Common;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.ContactMethods.Commands;

/// <summary>
/// Handles creation of a new contact method.
/// </summary>
public class CreateContactMethodCommandHandler : IRequestHandler<CreateContactMethodCommand, BaseResponse<ContactMethodDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateContactMethodCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public CreateContactMethodCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the creation of a new contact method.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created contact method response.</returns>
    public async Task<BaseResponse<ContactMethodDto>> Handle(
        CreateContactMethodCommand request,
        CancellationToken cancellationToken)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists)
        {
            return BaseResponse<ContactMethodDto>.Fail("User does not exist.");
        }

        var type = request.Type!.Trim();
        var value = request.Value!.Trim();
        var normalizedValue = value.ToUpperInvariant();

        var exists = await _context.ContactMethods.AnyAsync(
            c => c.UserId == request.UserId && c.Type == type && c.NormalizedValue == normalizedValue,
            cancellationToken);

        if (exists)
        {
            return BaseResponse<ContactMethodDto>.Fail("Contact method already exists.");
        }

        var contactMethod = new ContactMethod
        {
            UserId = request.UserId,
            Type = type,
            Value = value,
            NormalizedValue = normalizedValue,
            IsVerified = request.IsVerified,
            IsPrimary = request.IsPrimary
        };

        _context.ContactMethods.Add(contactMethod);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<ContactMethodDto>.Ok(new ContactMethodDto(contactMethod), $"Created contact method with id {contactMethod.Id}");
    }
}
