#nullable enable
using Application.Common.Interfaces;
using Application.ExternalLinks;
using Application.ExternalLinks.Models;
using Domain.Common;
using Domain.Constants;
using Domain.ValueObjects;
using Mediator;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.ExternalLinks.Queries;

/// <summary>
/// Query to retrieve a Google Calendar access token for a user.
/// </summary>
public sealed class GetGoogleCalendarAccessTokenQuery : IRequest<BaseResponse<ExternalAccessTokenDto>>
{
    /// <summary>
    /// Gets or sets the user identifier for which to retrieve the access token.
    /// </summary>
    public Guid UserId { get; set; }
}

/// <summary>
/// Handles retrieval of Google Calendar access tokens for a user.
/// </summary>
public sealed class GetGoogleCalendarAccessTokenQueryHandler
    : IRequestHandler<GetGoogleCalendarAccessTokenQuery, BaseResponse<ExternalAccessTokenDto>>
{
    private const int RefreshSkewSeconds = 60;

    private readonly IApplicationDbContext _context;
    private readonly IGoogleOAuthService _googleOAuthService;
    private readonly IExternalTokenProtector _tokenProtector;
    private readonly IClockService _clockService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetGoogleCalendarAccessTokenQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="googleOAuthService">The Google OAuth service.</param>
    /// <param name="tokenProtector">The token protector.</param>
    /// <param name="clockService">The clock service.</param>
    public GetGoogleCalendarAccessTokenQueryHandler(
        IApplicationDbContext context,
        IGoogleOAuthService googleOAuthService,
        IExternalTokenProtector tokenProtector,
        IClockService clockService)
    {
        _context = context;
        _googleOAuthService = googleOAuthService;
        _tokenProtector = tokenProtector;
        _clockService = clockService;
    }

    /// <inheritdoc />
    public async Task<BaseResponse<ExternalAccessTokenDto>> Handle(
        GetGoogleCalendarAccessTokenQuery request,
        CancellationToken cancellationToken)
    {
        var provider = ExternalProvider.From(ExternalProviderNames.Google);
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
        if (user == null || user.IsDeleted || user.IsLocked)
        {
            return BaseResponse<ExternalAccessTokenDto>.Fail("User is not available.");
        }

        var token = await _context.ExternalTokens.FirstOrDefaultAsync(
            x => x.UserId == request.UserId && x.Provider == provider,
            cancellationToken);

        if (token == null || string.IsNullOrWhiteSpace(token.AccessToken))
        {
            return BaseResponse<ExternalAccessTokenDto>.Fail("Google Calendar is not linked.");
        }

        if (string.IsNullOrWhiteSpace(token.Scopes) ||
            !token.Scopes.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Contains(ExternalLinkConstants.GoogleCalendarScope, StringComparer.OrdinalIgnoreCase))
        {
            return BaseResponse<ExternalAccessTokenDto>.Fail("Google Calendar scope is missing.");
        }

        var now = _clockService.Now;
        var refreshThreshold = now + Duration.FromSeconds(RefreshSkewSeconds);
        if (token.ExpiresAt <= refreshThreshold)
        {
            if (string.IsNullOrWhiteSpace(token.RefreshToken))
            {
                return BaseResponse<ExternalAccessTokenDto>.Fail("Refresh token is missing.");
            }

            var rawRefreshToken = _tokenProtector.Unprotect(token.RefreshToken);
            var refreshed = await _googleOAuthService.RefreshTokenAsync(rawRefreshToken, cancellationToken);

            if (string.IsNullOrWhiteSpace(refreshed.AccessToken))
            {
                return BaseResponse<ExternalAccessTokenDto>.Fail("Failed to refresh access token.");
            }

            token.AccessToken = _tokenProtector.Protect(refreshed.AccessToken);
            token.ExpiresAt = _clockService.Now + Duration.FromSeconds(refreshed.ExpiresInSeconds);
            token.UpdatedAt = _clockService.Now;

            if (!string.IsNullOrWhiteSpace(refreshed.Scope))
            {
                token.Scopes = refreshed.Scope;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        var response = new ExternalAccessTokenDto
        {
            AccessToken = _tokenProtector.Unprotect(token.AccessToken),
            ExpiresAtUtc = token.ExpiresAt.ToDateTimeUtc()
        };

        return BaseResponse<ExternalAccessTokenDto>.Ok(response);
    }
}
