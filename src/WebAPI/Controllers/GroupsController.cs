#nullable enable
using Application.Common.Models;
using Application.Groups.Commands;
using Application.Groups.Models;
using Application.Groups.Queries;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// API controller for managing groups.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class GroupsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    public GroupsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a paginated list of groups based on the specified query parameters.
    /// </summary>
    /// <param name="query">The query parameters for filtering, sorting, and paginating groups.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a paginated list of <see cref="GroupDto"/> objects.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<BaseResponse<PaginatedEnumerable<GroupDto>>>> GetGroups([FromQuery] GetGroupsQuery query)
        => Ok(await _mediator.Send(query));

    /// <summary>
    /// Retrieves a group by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the group to retrieve.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the <see cref="GroupDto"/> if found; otherwise, an error response.
    /// </returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<GroupDto>>> GetGroupById(Guid id)
        => Ok(await _mediator.Send(new GetGroupByIdQuery { Id = id }));

    /// <summary>
    /// Creates a new group with the specified details.
    /// </summary>
    /// <param name="command">The command containing the group details to create.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the created <see cref="GroupDto"/> object.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<BaseResponse<GroupDto>>> CreateGroup([FromBody] CreateGroupCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetGroupById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Updates an existing group with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier of the group to update.</param>
    /// <param name="command">The command containing the updated group details.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the updated <see cref="GroupDto"/> object.
    /// </returns>
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<BaseResponse<GroupDto>>> UpdateGroup(Guid id, [FromBody] UpdateGroupCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a group by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the group to delete.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a confirmation message if the operation was successful; otherwise, an error response.
    /// </returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse<string>>> DeleteGroup(Guid id)
        => Ok(await _mediator.Send(new DeleteGroupCommand { Id = id }));
}
