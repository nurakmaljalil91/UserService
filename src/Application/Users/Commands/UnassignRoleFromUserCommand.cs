#nullable enable
using Application.Users.Models;
using Domain.Common;
using Mediator;

namespace Application.Users.Commands;

/// <summary>
/// Command to unassign a role from a user.
/// </summary>
public class UnassignRoleFromUserCommand : IRequest<BaseResponse<UserDto>>
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the role identifier.
    /// </summary>
    public Guid RoleId { get; set; }
}
