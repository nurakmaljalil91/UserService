#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Groups.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Groups.Commands;

/// <summary>
/// Handles updating a group.
/// </summary>
public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, BaseResponse<GroupDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateGroupCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UpdateGroupCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the group update request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated group response.</returns>
    public async Task<BaseResponse<GroupDto>> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.Groups
            .Include(g => g.GroupRoles)
            .ThenInclude(gr => gr.Role)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (group == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Group), request.Id.ToString());
        }

        if (request.Name != null)
        {
            var name = request.Name.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                return BaseResponse<GroupDto>.Fail("Group name is required.");
            }

            var normalizedName = name.ToUpperInvariant();
            var exists = await _context.Groups.AnyAsync(
                g => g.Id != group.Id && g.NormalizedName == normalizedName,
                cancellationToken);

            if (exists)
            {
                return BaseResponse<GroupDto>.Fail("Group name already exists.");
            }

            group.Name = name;
            group.NormalizedName = normalizedName;
        }

        if (request.Description != null)
        {
            group.Description = request.Description.Trim();
        }

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<GroupDto>.Ok(new GroupDto(group), "Group updated.");
    }
}
