#nullable enable
using System.Linq;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.UserPreferences.Models;
using Application.UserProfiles.Models;
using Application.UserSessions.Models;
using Application.Users.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.UserSessions.Queries;

/// <summary>
/// Handles retrieval of the current user's session details.
/// </summary>
public class GetUserSessionQueryHandler : IRequestHandler<GetUserSessionQuery, BaseResponse<UserSessionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserSessionQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="user">The current user accessor.</param>
    public GetUserSessionQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    /// <summary>
    /// Handles the user session lookup request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user session response.</returns>
    public async Task<BaseResponse<UserSessionDto>> Handle(GetUserSessionQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.UserId;
        if (userId is null)
        {
            throw new UnauthorizedAccessException();
        }

        var user = await _context.Users
            .Include(u => u.Profile)
            .Include(u => u.UserPreferences)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role!)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserGroups)
            .ThenInclude(ug => ug.Group!)
            .ThenInclude(g => g.GroupRoles)
            .ThenInclude(gr => gr.Role!)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId.Value && !u.IsDeleted, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.User), userId.Value.ToString());
        }

        if (user.Profile == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.UserProfile), userId.Value.ToString());
        }

        var userDto = new UserDto(user);
        var profileDto = new UserProfileDto(user.Profile);
        var preferences = user.UserPreferences
            .Select(preference => new UserPreferenceDto(preference))
            .ToList();

        var session = new UserSessionDto(userDto, profileDto, preferences);

        return BaseResponse<UserSessionDto>.Ok(session, "User session retrieved.");
    }
}
