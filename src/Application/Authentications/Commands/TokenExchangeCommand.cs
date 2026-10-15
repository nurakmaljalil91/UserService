#nullable enable
using Application.Authentications.Models;
using Application.Common.Interfaces;
using Application.UserPreferences;
using Domain.Common;
using Domain.ValueObjects;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NodaTime;

namespace Application.Authentications.Commands;

/// <summary>
/// Exchanges a personal access token for a short-lived, scoped access token.
/// </summary>
public class TokenExchangeCommand : IRequest<BaseResponse<TokenExchangeResponse>>
{
    /// <summary>
    /// Gets or sets the personal access token secret.
    /// </summary>
    public string? Token { get; set; }
}

/// <summary>
/// Handles <see cref="TokenExchangeCommand"/>. Every failure returns the same message so callers cannot tell
/// unknown, revoked and expired tokens apart.
/// </summary>
public class TokenExchangeCommandHandler : IRequestHandler<TokenExchangeCommand, BaseResponse<TokenExchangeResponse>>
{
    /// <summary>
    /// The message returned for any token that cannot be exchanged.
    /// </summary>
    public const string InvalidTokenMessage = "Personal access token is invalid, expired or revoked.";

    private static readonly Duration LastUsedResolution = Duration.FromMinutes(1);

    private readonly IApplicationDbContext _context;
    private readonly IDateTime _dateTime;
    private readonly IClockService _clock;
    private readonly IConfiguration _configuration;
    private readonly IRefreshTokenHasher _hasher;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenExchangeCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="dateTime">The date/time provider used for JWT expiry.</param>
    /// <param name="clock">The clock service.</param>
    /// <param name="configuration">The application configuration (JWT settings).</param>
    /// <param name="hasher">The SHA-256 hasher used for token secrets.</param>
    public TokenExchangeCommandHandler(
        IApplicationDbContext context,
        IDateTime dateTime,
        IClockService clock,
        IConfiguration configuration,
        IRefreshTokenHasher hasher)
    {
        _context = context;
        _dateTime = dateTime;
        _clock = clock;
        _configuration = configuration;
        _hasher = hasher;
    }

    /// <summary>
    /// Validates the personal access token and issues a scoped access token.
    /// </summary>
    /// <param name="request">The command.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The access token, or a failure with <see cref="InvalidTokenMessage"/>.</returns>
    public async Task<BaseResponse<TokenExchangeResponse>> Handle(TokenExchangeCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return BaseResponse<TokenExchangeResponse>.Fail(InvalidTokenMessage);
        }

        var hash = _hasher.Hash(request.Token.Trim());
        var token = await _context.PersonalAccessTokens
            .Include(t => t.User)
            .ThenInclude(u => u!.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);

        var now = _clock.Now;
        if (token?.User is not { } user || !token.IsActive(now) || user.IsDeleted || user.IsLocked)
        {
            return BaseResponse<TokenExchangeResponse>.Fail(InvalidTokenMessage);
        }

        var scopes = TokenScope.Expand(token.Scopes.Where(TokenScope.IsValid));
        var timeZone = await TimeZonePreference.GetForUserAsync(_context, user.Id, cancellationToken);
        var jwt = JwtTokenFactory.Create(user, _configuration, _dateTime, timeZone, new PersonalAccessTokenGrant(token.Id, scopes));
        if (!jwt.Success)
        {
            return BaseResponse<TokenExchangeResponse>.Fail(jwt.Error ?? "JWT configuration is missing.");
        }

        if (token.LastUsedAt is null || now - token.LastUsedAt.Value >= LastUsedResolution)
        {
            token.LastUsedAt = now;
            await _context.SaveChangesAsync(cancellationToken);
        }

        return BaseResponse<TokenExchangeResponse>.Ok(
            new TokenExchangeResponse(jwt.Token, jwt.ExpiresAt, scopes),
            "Token exchanged.");
    }
}

/// <summary>
/// Validates <see cref="TokenExchangeCommand"/>.
/// </summary>
public class TokenExchangeCommandValidator : AbstractValidator<TokenExchangeCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenExchangeCommandValidator"/> class.
    /// </summary>
    public TokenExchangeCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required.");
    }
}
