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
/// Handles assigning a user to a group.
/// </summary>
public class AssignUserToGroupCommandHandler : IRequestHandler<AssignUserToGroupCommand, BaseResponse<GroupDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignUserToGroupCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public AssignUserToGroupCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the user assignment request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated group response.</returns>
    public async Task<BaseResponse<GroupDto>> Handle(AssignUserToGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.Groups
            .Include(g => g.GroupRoles)
            .ThenInclude(gr => gr.Role)
            .Include(g => g.UserGroups)
            .ThenInclude(ug => ug.User)
            .FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);

        if (group == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Group), request.GroupId.ToString());
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.User), request.UserId.ToString());
        }

        var alreadyAssigned = group.UserGroups.Any(ug => ug.UserId == user.Id);
        if (alreadyAssigned)
        {
            return BaseResponse<GroupDto>.Fail("User already assigned.");
        }

        group.UserGroups.Add(new UserGroup
        {
            GroupId = group.Id,
            UserId = user.Id,
            User = user
        });

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<GroupDto>.Ok(new GroupDto(group), "User assigned to group.");
    }
}
