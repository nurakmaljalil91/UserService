#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Users.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands;

/// <summary>
/// Handles unassigning a role from a user.
/// </summary>
public class UnassignRoleFromUserCommandHandler : IRequestHandler<UnassignRoleFromUserCommand, BaseResponse<UserDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnassignRoleFromUserCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UnassignRoleFromUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the role unassignment request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated user response.</returns>
    public async Task<BaseResponse<UserDto>> Handle(UnassignRoleFromUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r!.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.User), request.UserId.ToString());
        }

        var roleExists = await _context.Roles
            .AnyAsync(r => r.Id == request.RoleId, cancellationToken);

        if (!roleExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.Role), request.RoleId.ToString());
        }

        var userRole = user.UserRoles.FirstOrDefault(ur => ur.RoleId == request.RoleId);
        if (userRole == null)
        {
            return BaseResponse<UserDto>.Fail("Role is not assigned to user.");
        }

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync(cancellationToken);

        user.UserRoles.Remove(userRole);

        return BaseResponse<UserDto>.Ok(new UserDto(user), "Role unassigned from user.");
    }
}
