#nullable enable
using Application.Roles.Models;
using Domain.Common;
using Mediator;

namespace Application.Roles.Commands;

/// <summary>
/// Command to assign a permission to a role.
/// </summary>
public class AssignPermissionToRoleCommand : IRequest<BaseResponse<RoleDto>>
{
    /// <summary>
    /// Gets or sets the role identifier.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Gets or sets the permission identifier.
    /// </summary>
    public Guid PermissionId { get; set; }
}
