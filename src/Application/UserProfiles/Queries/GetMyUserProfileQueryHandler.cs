#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.UserProfiles.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.UserProfiles.Queries;

/// <summary>
/// Handles retrieval of the current user's profile.
/// </summary>
public class GetMyUserProfileQueryHandler : IRequestHandler<GetMyUserProfileQuery, BaseResponse<UserProfileDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMyUserProfileQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="user">The current user accessor.</param>
    public GetMyUserProfileQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    /// <summary>
    /// Handles the user profile lookup for the current user.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user profile response.</returns>
    public async Task<BaseResponse<UserProfileDto>> Handle(GetMyUserProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.UserId;
        if (userId is null)
        {
            throw new UnauthorizedAccessException();
        }

        var profile = await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId.Value, cancellationToken);

        if (profile == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.UserProfile), userId.Value.ToString());
        }

        return BaseResponse<UserProfileDto>.Ok(new UserProfileDto(profile), "User profile retrieved.");
    }
}
