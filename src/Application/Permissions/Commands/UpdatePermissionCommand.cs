#nullable enable
using Application.Permissions.Models;
using Domain.Common;
using Mediator;

namespace Application.Permissions.Commands;

/// <summary>
/// Command to update an existing permission.
/// </summary>
public class UpdatePermissionCommand : IRequest<BaseResponse<PermissionDto>>
{
    /// <summary>
    /// Gets or sets the permission identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the permission name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the permission description.
    /// </summary>
    public string? Description { get; set; }
}
