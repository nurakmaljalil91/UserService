#nullable enable
using Application.Groups.Models;
using Domain.Common;
using Mediator;

namespace Application.Groups.Commands;

/// <summary>
/// Command to create a new group.
/// </summary>
public class CreateGroupCommand : IRequest<BaseResponse<GroupDto>>
{
    /// <summary>
    /// Gets or sets the group name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the group description.
    /// </summary>
    public string? Description { get; set; }
}
