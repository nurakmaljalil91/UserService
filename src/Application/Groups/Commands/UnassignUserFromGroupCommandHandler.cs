#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Groups.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Groups.Commands;

/// <summary>
/// Handles unassigning a user from a group.
/// </summary>
public class UnassignUserFromGroupCommandHandler : IRequestHandler<UnassignUserFromGroupCommand, BaseResponse<GroupDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnassignUserFromGroupCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UnassignUserFromGroupCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the user unassignment request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated group response.</returns>
    public async Task<BaseResponse<GroupDto>> Handle(UnassignUserFromGroupCommand request, CancellationToken cancellationToken)
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

        var userExists = await _context.Users
            .AnyAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

        if (!userExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.User), request.UserId.ToString());
        }

        var userGroup = group.UserGroups.FirstOrDefault(ug => ug.UserId == request.UserId);
        if (userGroup == null)
        {
            return BaseResponse<GroupDto>.Fail("User is not assigned to group.");
        }

        _context.UserGroups.Remove(userGroup);
        await _context.SaveChangesAsync(cancellationToken);

        group.UserGroups.Remove(userGroup);

        return BaseResponse<GroupDto>.Ok(new GroupDto(group), "User unassigned from group.");
    }
}
