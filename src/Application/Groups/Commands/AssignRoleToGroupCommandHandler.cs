#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Groups.Models;
using Domain.Common;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Groups.Commands;

/// <summary>
/// Handles assigning a role to a group.
/// </summary>
public class AssignRoleToGroupCommandHandler : IRequestHandler<AssignRoleToGroupCommand, BaseResponse<GroupDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignRoleToGroupCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public AssignRoleToGroupCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the role assignment request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated group response.</returns>
    public async Task<BaseResponse<GroupDto>> Handle(AssignRoleToGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.Groups
            .Include(g => g.GroupRoles)
            .ThenInclude(gr => gr.Role)
            .FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);

        if (group == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Group), request.GroupId.ToString());
        }

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);
        if (role == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Role), request.RoleId.ToString());
        }

        var alreadyAssigned = group.GroupRoles.Any(gr => gr.RoleId == role.Id);
        if (alreadyAssigned)
        {
            return BaseResponse<GroupDto>.Fail("Role already assigned.");
        }

        group.GroupRoles.Add(new GroupRole
        {
            GroupId = group.Id,
            RoleId = role.Id,
            Role = role
        });

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<GroupDto>.Ok(new GroupDto(group), "Role assigned to group.");
    }
}
