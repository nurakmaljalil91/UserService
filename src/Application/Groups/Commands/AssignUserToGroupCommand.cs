#nullable enable
using Application.Groups.Models;
using Domain.Common;
using Mediator;

namespace Application.Groups.Commands;

/// <summary>
/// Command to assign a user to a group.
/// </summary>
public class AssignUserToGroupCommand : IRequest<BaseResponse<GroupDto>>
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
