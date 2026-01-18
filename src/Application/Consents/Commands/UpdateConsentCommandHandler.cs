#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Consents.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;
using NodaTime.Text;

namespace Application.Consents.Commands;

/// <summary>
/// Handles updating a consent.
/// </summary>
public class UpdateConsentCommandHandler : IRequestHandler<UpdateConsentCommand, BaseResponse<ConsentDto>>
{
    private const string InstantPatternText = "yyyy-MM-dd'T'HH:mm:ss'Z'";
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateConsentCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UpdateConsentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the consent update request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated consent response.</returns>
    public async Task<BaseResponse<ConsentDto>> Handle(UpdateConsentCommand request, CancellationToken cancellationToken)
    {
        var consent = await _context.Consents.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (consent == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Consent), request.Id.ToString());
        }

        if (request.Type != null)
        {
            var type = request.Type.Trim();
            if (string.IsNullOrWhiteSpace(type))
            {
                return BaseResponse<ConsentDto>.Fail("Consent type is required.");
            }

            consent.Type = type;
        }

        if (request.IsGranted.HasValue)
        {
            consent.IsGranted = request.IsGranted.Value;
        }

        if (request.GrantedAt != null)
        {
            if (string.IsNullOrWhiteSpace(request.GrantedAt))
            {
                return BaseResponse<ConsentDto>.Fail($"GrantedAt must be in {InstantPatternText} format.");
            }

            var instantPattern = InstantPattern.CreateWithInvariantCulture(InstantPatternText);
            var parsed = instantPattern.Parse(request.GrantedAt);
            if (!parsed.Success)
            {
                return BaseResponse<ConsentDto>.Fail($"GrantedAt must be in {InstantPatternText} format.");
            }

            consent.GrantedAt = parsed.Value;
        }

        if (request.Version != null)
        {
            consent.Version = request.Version.Trim();
        }

        if (request.Source != null)
        {
            consent.Source = request.Source.Trim();
        }

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<ConsentDto>.Ok(new ConsentDto(consent), "Consent updated.");
    }
}
