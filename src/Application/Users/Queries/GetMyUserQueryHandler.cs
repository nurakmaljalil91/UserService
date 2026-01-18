#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Users.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries;

/// <summary>
/// Handles retrieval of the current user.
/// </summary>
public class GetMyUserQueryHandler : IRequestHandler<GetMyUserQuery, BaseResponse<UserDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMyUserQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="user">The current user accessor.</param>
    public GetMyUserQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    /// <summary>
    /// Handles the current user lookup request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user response.</returns>
    public async Task<BaseResponse<UserDto>> Handle(GetMyUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.UserId;
        if (userId is null)
        {
            throw new UnauthorizedAccessException();
        }

        var user = await _context.Users
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

        return BaseResponse<UserDto>.Ok(new UserDto(user), "User retrieved.");
    }
}
