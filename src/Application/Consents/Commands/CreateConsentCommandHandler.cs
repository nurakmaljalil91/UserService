#nullable enable
using Application.Common.Interfaces;
using Application.Consents.Models;
using Domain.Common;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Text;

namespace Application.Consents.Commands;

/// <summary>
/// Handles creation of a new consent.
/// </summary>
public class CreateConsentCommandHandler : IRequestHandler<CreateConsentCommand, BaseResponse<ConsentDto>>
{
    private const string InstantPatternText = "yyyy-MM-dd'T'HH:mm:ss'Z'";
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateConsentCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public CreateConsentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the creation of a new consent.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created consent response.</returns>
    public async Task<BaseResponse<ConsentDto>> Handle(CreateConsentCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists)
        {
            return BaseResponse<ConsentDto>.Fail("User does not exist.");
        }

        var type = request.Type!.Trim();
        var grantedAt = TryParseInstant(request.GrantedAt, out var grantedError);
        if (grantedError != null)
        {
            return BaseResponse<ConsentDto>.Fail(grantedError);
        }

        var consent = new Consent
        {
            UserId = request.UserId,
            Type = type,
            IsGranted = request.IsGranted,
            GrantedAt = grantedAt,
            Version = request.Version?.Trim(),
            Source = request.Source?.Trim()
        };

        _context.Consents.Add(consent);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<ConsentDto>.Ok(new ConsentDto(consent), $"Created consent with id {consent.Id}");
    }

    /// <summary>
    /// Attempts to parse a required instant value from a UTC string.
    /// </summary>
    /// <param name="value">The instant string to parse.</param>
    /// <param name="error">The error message when parsing fails.</param>
    /// <returns>The parsed instant value.</returns>
    private static Instant TryParseInstant(string? value, out string? error)
    {
        error = null;
        var instantPattern = InstantPattern.CreateWithInvariantCulture(InstantPatternText);

        if (string.IsNullOrWhiteSpace(value))
        {
            error = $"GrantedAt is required and must be in {InstantPatternText} format.";
            return default;
        }

        var parsed = instantPattern.Parse(value);
        if (!parsed.Success)
        {
            error = $"GrantedAt must be in {InstantPatternText} format.";
            return default;
        }

        return parsed.Value;
    }
}
