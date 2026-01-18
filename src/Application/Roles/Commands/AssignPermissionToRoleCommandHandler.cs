#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Roles.Models;
using Domain.Common;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.Commands;

/// <summary>
/// Handles assigning a permission to a role.
/// </summary>
public class AssignPermissionToRoleCommandHandler : IRequestHandler<AssignPermissionToRoleCommand, BaseResponse<RoleDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignPermissionToRoleCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public AssignPermissionToRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the permission assignment request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated role response.</returns>
    public async Task<BaseResponse<RoleDto>> Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

        if (role == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Role), request.RoleId.ToString());
        }

        var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.Id == request.PermissionId, cancellationToken);
        if (permission == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Permission), request.PermissionId.ToString());
        }

        var alreadyAssigned = role.RolePermissions.Any(rp => rp.PermissionId == permission.Id);
        if (alreadyAssigned)
        {
            return BaseResponse<RoleDto>.Fail("Permission already assigned.");
        }

        role.RolePermissions.Add(new RolePermission
        {
            RoleId = role.Id,
            PermissionId = permission.Id,
            Permission = permission
        });

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<RoleDto>.Ok(new RoleDto(role), "Permission assigned to role.");
    }
}
