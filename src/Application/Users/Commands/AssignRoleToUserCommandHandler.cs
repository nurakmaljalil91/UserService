#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Users.Models;
using Domain.Common;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands;

/// <summary>
/// Handles assigning a role to a user.
/// </summary>
public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand, BaseResponse<UserDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignRoleToUserCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public AssignRoleToUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the role assignment request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated user response.</returns>
    public async Task<BaseResponse<UserDto>> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
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

        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

        if (role == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Role), request.RoleId.ToString());
        }

        var alreadyAssigned = user.UserRoles.Any(ur => ur.RoleId == role.Id);
        if (alreadyAssigned)
        {
            return BaseResponse<UserDto>.Fail("Role already assigned.");
        }

        user.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id,
            Role = role
        });

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<UserDto>.Ok(new UserDto(user), "Role assigned to user.");
    }
}
