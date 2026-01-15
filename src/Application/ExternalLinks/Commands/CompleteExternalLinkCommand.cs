#nullable enable
using Application.Common.Interfaces;
using Application.ExternalLinks;
using Application.ExternalLinks.Models;
using Domain.Common;
using Domain.Constants;
using Domain.Entities;
using Domain.ValueObjects;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Mediator;

namespace Application.ExternalLinks.Commands;

/// <summary>
/// Command to complete an external account link flow.
/// </summary>
public sealed class CompleteExternalLinkCommand : IRequest<BaseResponse<ExternalLinkDto>>
{
    /// <summary>
    /// Gets or sets the external provider name.
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    /// Gets or sets the authorization code from the provider.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets the state value returned by the provider.
    /// </summary>
    public string? State { get; set; }
}

/// <summary>
/// Handles completion of an external account link flow.
/// </summary>
public sealed class CompleteExternalLinkCommandHandler : IRequestHandler<CompleteExternalLinkCommand, BaseResponse<ExternalLinkDto>>
{
    private const string MissingRefreshTokenMessage = "Refresh token is required to link the external account.";

    private readonly IApplicationDbContext _context;
    private readonly IExternalLinkStateService _stateService;
    private readonly IGoogleOAuthService _googleOAuthService;
    private readonly IExternalTokenProtector _tokenProtector;
    private readonly IClockService _clockService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompleteExternalLinkCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="stateService">The external link state service.</param>
    /// <param name="googleOAuthService">The Google OAuth service.</param>
    /// <param name="tokenProtector">The token protector.</param>
    /// <param name="clockService">The clock service.</param>
    public CompleteExternalLinkCommandHandler(
        IApplicationDbContext context,
        IExternalLinkStateService stateService,
        IGoogleOAuthService googleOAuthService,
        IExternalTokenProtector tokenProtector,
        IClockService clockService)
    {
        _context = context;
        _stateService = stateService;
        _googleOAuthService = googleOAuthService;
        _tokenProtector = tokenProtector;
        _clockService = clockService;
    }

    /// <inheritdoc />
    public async Task<BaseResponse<ExternalLinkDto>> Handle(
        CompleteExternalLinkCommand request,
        CancellationToken cancellationToken)
    {
        var providerValue = string.IsNullOrWhiteSpace(request.Provider)
            ? ExternalProviderNames.Google
            : request.Provider.Trim();

        if (!string.Equals(providerValue, ExternalProviderNames.Google, StringComparison.OrdinalIgnoreCase))
        {
            return BaseResponse<ExternalLinkDto>.Fail("Unsupported external provider.");
        }

        var validation = _stateService.ValidateState(request.State ?? string.Empty);
        if (!validation.IsValid || validation.UserId == null || validation.Provider == null)
        {
            return BaseResponse<ExternalLinkDto>.Fail("Invalid state value.");
        }

        if (!string.Equals(validation.Provider.Value, providerValue, StringComparison.OrdinalIgnoreCase))
        {
            return BaseResponse<ExternalLinkDto>.Fail("Provider mismatch.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == validation.UserId.Value, cancellationToken);
        if (user == null || user.IsDeleted || user.IsLocked)
        {
            return BaseResponse<ExternalLinkDto>.Fail("User is not available.");
        }

        var tokenResponse = await _googleOAuthService.ExchangeCodeAsync(request.Code ?? string.Empty, cancellationToken);
        if (string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
        {
            return BaseResponse<ExternalLinkDto>.Fail("Access token was not returned by the provider.");
        }

        var profile = await _googleOAuthService.GetUserProfileAsync(tokenResponse.AccessToken, cancellationToken);
        if (string.IsNullOrWhiteSpace(profile.SubjectId))
        {
            return BaseResponse<ExternalLinkDto>.Fail("External profile information is missing.");
        }

        var provider = ExternalProvider.From(providerValue);
        var subjectId = ExternalSubjectId.From(profile.SubjectId);

        var existingForSubject = await _context.ExternalIdentities
            .FirstOrDefaultAsync(
                x => x.Provider == provider && x.SubjectId == subjectId,
                cancellationToken);

        if (existingForSubject != null && existingForSubject.UserId != user.Id)
        {
            return BaseResponse<ExternalLinkDto>.Fail("External account is already linked to another user.");
        }

        var existingIdentity = await _context.ExternalIdentities
            .FirstOrDefaultAsync(
                x => x.UserId == user.Id && x.Provider == provider,
                cancellationToken);

        if (existingIdentity != null && existingIdentity.SubjectId != subjectId)
        {
            return BaseResponse<ExternalLinkDto>.Fail("A different external account is already linked.");
        }

        if (existingIdentity == null)
        {
            existingIdentity = new ExternalIdentity
            {
                UserId = user.Id,
                Provider = provider,
                SubjectId = subjectId,
                LinkedAt = _clockService.Now
            };
            _context.ExternalIdentities.Add(existingIdentity);
        }

        existingIdentity.Email = profile.Email;
        existingIdentity.DisplayName = profile.DisplayName;

        var existingToken = await _context.ExternalTokens
            .FirstOrDefaultAsync(
                x => x.UserId == user.Id && x.Provider == provider,
                cancellationToken);

        var refreshToken = tokenResponse.RefreshToken;
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            if (existingToken != null && !string.IsNullOrWhiteSpace(existingToken.RefreshToken))
            {
                refreshToken = _tokenProtector.Unprotect(existingToken.RefreshToken);
            }
            else
            {
                return BaseResponse<ExternalLinkDto>.Fail(MissingRefreshTokenMessage);
            }
        }

        var expiresAt = _clockService.Now + Duration.FromSeconds(tokenResponse.ExpiresInSeconds);
        var scopes = string.IsNullOrWhiteSpace(tokenResponse.Scope)
            ? existingToken?.Scopes ?? ExternalLinkConstants.GoogleCalendarScope
            : tokenResponse.Scope!;

        if (existingToken == null)
        {
            existingToken = new ExternalToken
            {
                UserId = user.Id,
                Provider = provider
            };
            _context.ExternalTokens.Add(existingToken);
        }

        existingToken.AccessToken = _tokenProtector.Protect(tokenResponse.AccessToken);
        existingToken.RefreshToken = _tokenProtector.Protect(refreshToken);
        existingToken.ExpiresAt = expiresAt;
        existingToken.Scopes = scopes;
        existingToken.UpdatedAt = _clockService.Now;

        await _context.SaveChangesAsync(cancellationToken);

        var response = new ExternalLinkDto
        {
            Provider = provider.Value,
            SubjectId = subjectId.Value,
            Email = existingIdentity.Email,
            DisplayName = existingIdentity.DisplayName,
            LinkedAtUtc = existingIdentity.LinkedAt.ToDateTimeUtc()
        };

        return BaseResponse<ExternalLinkDto>.Ok(response, "External link completed.");
    }
}

/// <summary>
/// Validates the <see cref="CompleteExternalLinkCommand"/>.
/// </summary>
public sealed class CompleteExternalLinkCommandValidator : AbstractValidator<CompleteExternalLinkCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompleteExternalLinkCommandValidator"/> class.
    /// </summary>
    public CompleteExternalLinkCommandValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty()
            .WithMessage("Provider is required.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Authorization code is required.");

        RuleFor(x => x.State)
            .NotEmpty()
            .WithMessage("State is required.");
    }
}
