#nullable enable
using Application.Common.Interfaces;
using Application.Sessions.Models;
using Domain.Common;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Text;

namespace Application.Sessions.Commands;

/// <summary>
/// Handles creation of a new session.
/// </summary>
public class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, BaseResponse<SessionDto>>
{
    private const string InstantPatternText = "yyyy-MM-dd'T'HH:mm:ss'Z'";
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSessionCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public CreateSessionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the creation of a new session.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created session response.</returns>
    public async Task<BaseResponse<SessionDto>> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists)
        {
            return BaseResponse<SessionDto>.Fail("User does not exist.");
        }

        var refreshToken = request.RefreshToken!.Trim();
        var tokenExists = await _context.Sessions.AnyAsync(s => s.RefreshToken == refreshToken, cancellationToken);
        if (tokenExists)
        {
            return BaseResponse<SessionDto>.Fail("Refresh token already exists.");
        }

        var expiresAt = TryParseInstant(request.ExpiresAt, out var expiresError);
        if (expiresError != null)
        {
            return BaseResponse<SessionDto>.Fail(expiresError);
        }

        var revokedAt = TryParseOptionalInstant(request.RevokedAt, out var revokedError);
        if (revokedError != null)
        {
            return BaseResponse<SessionDto>.Fail(revokedError);
        }

        var session = new Session
        {
            UserId = request.UserId,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            RevokedAt = revokedAt,
            IpAddress = request.IpAddress?.Trim(),
            UserAgent = request.UserAgent?.Trim(),
            DeviceName = request.DeviceName?.Trim(),
            IsRevoked = revokedAt.HasValue
        };

        _context.Sessions.Add(session);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<SessionDto>.Ok(new SessionDto(session), $"Created session with id {session.Id}");
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
            error = $"ExpiresAt is required and must be in {InstantPatternText} format.";
            return default;
        }

        var parsed = instantPattern.Parse(value);
        if (!parsed.Success)
        {
            error = $"ExpiresAt must be in {InstantPatternText} format.";
            return default;
        }

        return parsed.Value;
    }

    /// <summary>
    /// Attempts to parse an optional instant value from a UTC string.
    /// </summary>
    /// <param name="value">The instant string to parse.</param>
    /// <param name="error">The error message when parsing fails.</param>
    /// <returns>The parsed instant value, or null when input is empty.</returns>
    private static Instant? TryParseOptionalInstant(string? value, out string? error)
    {
        error = null;
        var instantPattern = InstantPattern.CreateWithInvariantCulture(InstantPatternText);

        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var parsed = instantPattern.Parse(value);
        if (!parsed.Success)
        {
            error = $"RevokedAt must be in {InstantPatternText} format.";
            return null;
        }

        return parsed.Value;
    }
}
