#nullable enable
using Application.Authentications.Models;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NodaTime;
using System.Security.Cryptography;

namespace Application.Authentications.Commands;

/// <summary>
/// Command to refresh an access token using a refresh token.
/// </summary>
public class RefreshTokenCommand : IRequest<BaseResponse<LoginResponse>>
{
    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    public string? RefreshToken { get; set; }
}

/// <summary>
/// Handles refreshing access tokens.
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, BaseResponse<LoginResponse>>
{
    private const int DefaultRefreshExpiryDays = 30;
    private const string InvalidRefreshTokenMessage = "Refresh token is invalid or expired.";

    private readonly IApplicationDbContext _context;
    private readonly IDateTime _dateTime;
    private readonly IClockService _clockService;
    private readonly IConfiguration _configuration;
    private readonly IRefreshTokenHasher _refreshTokenHasher;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenCommandHandler"/> class.
    /// </summary>
    public RefreshTokenCommandHandler(
        IApplicationDbContext context,
        IDateTime dateTime,
        IClockService clockService,
        IConfiguration configuration,
        IRefreshTokenHasher refreshTokenHasher)
    {
        _context = context;
        _dateTime = dateTime;
        _clockService = clockService;
        _configuration = configuration;
        _refreshTokenHasher = refreshTokenHasher;
    }

    /// <inheritdoc />
    public async Task<BaseResponse<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return BaseResponse<LoginResponse>.Fail(InvalidRefreshTokenMessage);
        }

        var refreshTokenHash = _refreshTokenHasher.Hash(request.RefreshToken);
        var session = await _context.Sessions
            .Include(s => s.User)
            .ThenInclude(u => u!.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(s => s.RefreshToken == refreshTokenHash, cancellationToken);

        if (session == null || session.IsRevoked || session.User == null)
        {
            return BaseResponse<LoginResponse>.Fail(InvalidRefreshTokenMessage);
        }

        if (session.ExpiresAt <= _clockService.Now)
        {
            session.IsRevoked = true;
            session.RevokedAt = _clockService.Now;
            await _context.SaveChangesAsync(cancellationToken);
            return BaseResponse<LoginResponse>.Fail(InvalidRefreshTokenMessage);
        }

        var user = session.User;
        if (user.IsDeleted || user.IsLocked)
        {
            session.IsRevoked = true;
            session.RevokedAt = _clockService.Now;
            await _context.SaveChangesAsync(cancellationToken);
            return BaseResponse<LoginResponse>.Fail(InvalidRefreshTokenMessage);
        }

        var jwtToken = JwtTokenFactory.Create(user, _configuration, _dateTime);
        if (!jwtToken.Success)
        {
            return BaseResponse<LoginResponse>.Fail(jwtToken.Error ?? "JWT configuration is missing.");
        }

        var newRefreshToken = CreateRefreshToken();
        var newRefreshTokenHash = _refreshTokenHasher.Hash(newRefreshToken);
        var refreshExpiryDays = ResolveRefreshExpiryDays();
        var refreshExpiresAt = _clockService.Now + Duration.FromDays(refreshExpiryDays);

        session.RefreshToken = newRefreshTokenHash;
        session.ExpiresAt = refreshExpiresAt;
        session.IsRevoked = false;
        session.RevokedAt = null;

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<LoginResponse>.Ok(
            new LoginResponse(
                jwtToken.Token,
                jwtToken.ExpiresAt,
                newRefreshToken,
                refreshExpiresAt.ToDateTimeUtc()),
            "Token refreshed.");
    }

    private int ResolveRefreshExpiryDays()
    {
        var jwtSection = _configuration.GetSection("Jwt");
        if (int.TryParse(jwtSection["RefreshTokenExpiryDays"], out var configuredDays) && configuredDays > 0)
        {
            return configuredDays;
        }

        return DefaultRefreshExpiryDays;
    }

    private static string CreateRefreshToken()
    {
        Span<byte> bytes = stackalloc byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
}

/// <summary>
/// Validates the <see cref="RefreshTokenCommand"/>.
/// </summary>
public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenCommandValidator"/> class.
    /// </summary>
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}
