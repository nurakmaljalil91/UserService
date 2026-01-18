#nullable enable
using Application.Groups.Models;
using Domain.Common;
using Mediator;

namespace Application.Groups.Commands;

/// <summary>
/// Command to assign a role to a group.
/// </summary>
public class AssignRoleToGroupCommand : IRequest<BaseResponse<GroupDto>>
{
    /// <summary>
    /// Gets or sets the group identifier.
    /// </summary>
    public Guid GroupId { get; set; }

    /// <summary>
    /// Gets or sets the role identifier.
    /// </summary>
    public Guid RoleId { get; set; }
}
