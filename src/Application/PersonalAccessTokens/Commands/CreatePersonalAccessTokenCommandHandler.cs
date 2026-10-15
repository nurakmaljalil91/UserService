#nullable enable
using Application.Common.Interfaces;
using Application.PersonalAccessTokens.Models;
using Domain.Common;
using Domain.Entities;
using Domain.ValueObjects;
using Mediator;
using NodaTime;

namespace Application.PersonalAccessTokens.Commands;

/// <summary>
/// Handles <see cref="CreatePersonalAccessTokenCommand"/>.
/// </summary>
public class CreatePersonalAccessTokenCommandHandler
    : IRequestHandler<CreatePersonalAccessTokenCommand, BaseResponse<CreatedPersonalAccessTokenDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;
    private readonly IClockService _clock;
    private readonly IRefreshTokenHasher _hasher;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreatePersonalAccessTokenCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="user">The current authenticated user.</param>
    /// <param name="clock">The clock service.</param>
    /// <param name="hasher">The SHA-256 hasher used for token secrets.</param>
    public CreatePersonalAccessTokenCommandHandler(
        IApplicationDbContext context,
        IUser user,
        IClockService clock,
        IRefreshTokenHasher hasher)
    {
        _context = context;
        _user = user;
        _clock = clock;
        _hasher = hasher;
    }

    /// <summary>
    /// Creates the token and returns its secret once.
    /// </summary>
    /// <param name="request">The command.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The created token including its secret.</returns>
    public async Task<BaseResponse<CreatedPersonalAccessTokenDto>> Handle(
        CreatePersonalAccessTokenCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _user.UserId ?? throw new UnauthorizedAccessException();
        var secret = PersonalAccessTokenSecret.Create();

        var token = new PersonalAccessToken
        {
            UserId = userId,
            Name = request.Name!.Trim(),
            DisplayPrefix = PersonalAccessTokenSecret.DisplayPrefixOf(secret),
            TokenHash = _hasher.Hash(secret),
            Scopes = TokenScope.Expand(request.Scopes).ToList(),
            ExpiresAt = request.ExpiresAt.HasValue ? Instant.FromDateTimeOffset(request.ExpiresAt.Value) : null
        };

        _context.PersonalAccessTokens.Add(token);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<CreatedPersonalAccessTokenDto>.Ok(
            new CreatedPersonalAccessTokenDto(token, _clock.Now, secret),
            "Personal access token created. Copy it now; it will not be shown again.");
    }
}
