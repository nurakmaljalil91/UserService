#nullable enable
using Application.Groups.Models;
using Domain.Common;
using Mediator;

namespace Application.Groups.Commands;

/// <summary>
/// Command to unassign a user from a group.
/// </summary>
public class UnassignUserFromGroupCommand : IRequest<BaseResponse<GroupDto>>
{
    /// <summary>
    /// Gets or sets the group identifier.
    /// </summary>
    public Guid GroupId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }
}
