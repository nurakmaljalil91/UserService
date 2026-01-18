#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Sessions.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;
using NodaTime.Text;

namespace Application.Sessions.Commands;

/// <summary>
/// Handles updating a session.
/// </summary>
public class UpdateSessionCommandHandler : IRequestHandler<UpdateSessionCommand, BaseResponse<SessionDto>>
{
    private const string InstantPatternText = "yyyy-MM-dd'T'HH:mm:ss'Z'";
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSessionCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UpdateSessionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the session update request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated session response.</returns>
    public async Task<BaseResponse<SessionDto>> Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (session == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Session), request.Id.ToString());
        }

        if (request.RefreshToken != null)
        {
            var refreshToken = request.RefreshToken.Trim();
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return BaseResponse<SessionDto>.Fail("Refresh token is required.");
            }

            var exists = await _context.Sessions.AnyAsync(
                s => s.Id != session.Id && s.RefreshToken == refreshToken,
                cancellationToken);

            if (exists)
            {
                return BaseResponse<SessionDto>.Fail("Refresh token already exists.");
            }

            session.RefreshToken = refreshToken;
        }

        if (request.ExpiresAt != null)
        {
            if (string.IsNullOrWhiteSpace(request.ExpiresAt))
            {
                return BaseResponse<SessionDto>.Fail($"ExpiresAt must be in {InstantPatternText} format.");
            }

            var instantPattern = InstantPattern.CreateWithInvariantCulture(InstantPatternText);
            var parsed = instantPattern.Parse(request.ExpiresAt);
            if (!parsed.Success)
            {
                return BaseResponse<SessionDto>.Fail($"ExpiresAt must be in {InstantPatternText} format.");
            }

            session.ExpiresAt = parsed.Value;
        }

        if (request.RevokedAt != null)
        {
            if (string.IsNullOrWhiteSpace(request.RevokedAt))
            {
                session.RevokedAt = null;
            }
            else
            {
                var instantPattern = InstantPattern.CreateWithInvariantCulture(InstantPatternText);
                var parsed = instantPattern.Parse(request.RevokedAt);
                if (!parsed.Success)
                {
                    return BaseResponse<SessionDto>.Fail($"RevokedAt must be in {InstantPatternText} format.");
                }

                session.RevokedAt = parsed.Value;
            }
        }

        if (request.IpAddress != null)
        {
            session.IpAddress = request.IpAddress.Trim();
        }

        if (request.UserAgent != null)
        {
            session.UserAgent = request.UserAgent.Trim();
        }

        if (request.DeviceName != null)
        {
            session.DeviceName = request.DeviceName.Trim();
        }

        if (request.IsRevoked.HasValue)
        {
            session.IsRevoked = request.IsRevoked.Value;
            if (session.IsRevoked && session.RevokedAt == null)
            {
                session.RevokedAt = session.ExpiresAt;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<SessionDto>.Ok(new SessionDto(session), "Session updated.");
    }
}
