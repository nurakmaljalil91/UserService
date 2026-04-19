#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Groups.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Groups.Commands;

/// <summary>
/// Handles unassigning a role from a group.
/// </summary>
public class UnassignRoleFromGroupCommandHandler : IRequestHandler<UnassignRoleFromGroupCommand, BaseResponse<GroupDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnassignRoleFromGroupCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UnassignRoleFromGroupCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the role unassignment request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated group response.</returns>
    public async Task<BaseResponse<GroupDto>> Handle(UnassignRoleFromGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.Groups
            .Include(g => g.GroupRoles)
            .ThenInclude(gr => gr.Role)
            .FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);

        if (group == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Group), request.GroupId.ToString());
        }

        var roleExists = await _context.Roles
            .AnyAsync(r => r.Id == request.RoleId, cancellationToken);

        if (!roleExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.Role), request.RoleId.ToString());
        }

        var groupRole = group.GroupRoles.FirstOrDefault(gr => gr.RoleId == request.RoleId);
        if (groupRole == null)
        {
            return BaseResponse<GroupDto>.Fail("Role is not assigned to group.");
        }

        _context.GroupRoles.Remove(groupRole);
        await _context.SaveChangesAsync(cancellationToken);

        group.GroupRoles.Remove(groupRole);

        return BaseResponse<GroupDto>.Ok(new GroupDto(group), "Role unassigned from group.");
    }
}
