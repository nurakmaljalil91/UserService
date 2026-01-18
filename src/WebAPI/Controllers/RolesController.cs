#nullable enable
using Application.Common.Models;
using Application.Roles.Commands;
using Application.Roles.Models;
using Application.Roles.Queries;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// API controller for managing roles.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class RolesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="RolesController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    public RolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a paginated list of roles based on the specified query parameters.
    /// </summary>
    /// <param name="query">The query parameters for filtering, sorting, and paginating roles.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a paginated list of <see cref="RoleDto"/> objects.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<BaseResponse<PaginatedEnumerable<RoleDto>>>> GetRoles([FromQuery] GetRolesQuery query)
        => Ok(await _mediator.Send(query));

    /// <summary>
    /// Retrieves a paginated list of the current user's roles.
    /// </summary>
    /// <param name="query">The query parameters for filtering, sorting, and paginating roles.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a paginated list of <see cref="RoleDto"/> objects.
    /// </returns>
    [HttpGet("me")]
    public async Task<ActionResult<BaseResponse<PaginatedEnumerable<RoleDto>>>> GetMyRoles([FromQuery] GetMyRolesQuery query)
        => Ok(await _mediator.Send(query));

    /// <summary>
    /// Retrieves a role by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the role to retrieve.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the <see cref="RoleDto"/> if found; otherwise, an error response.
    /// </returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<RoleDto>>> GetRoleById(Guid id)
        => Ok(await _mediator.Send(new GetRoleByIdQuery { Id = id }));

    /// <summary>
    /// Creates a new role with the specified details.
    /// </summary>
    /// <param name="command">The command containing the role details to create.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the created <see cref="RoleDto"/> object.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<BaseResponse<RoleDto>>> CreateRole([FromBody] CreateRoleCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetRoleById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Updates an existing role with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier of the role to update.</param>
    /// <param name="command">The command containing the updated role details.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the updated <see cref="RoleDto"/> object.
    /// </returns>
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<BaseResponse<RoleDto>>> UpdateRole(Guid id, [FromBody] UpdateRoleCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Assigns a permission to a role.
    /// </summary>
    /// <param name="id">The unique identifier of the role to assign the permission to.</param>
    /// <param name="command">The command containing the permission assignment details.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the updated <see cref="RoleDto"/> object.
    /// </returns>
    [HttpPost("{id:guid}/assign-permission")]
    public async Task<ActionResult<BaseResponse<RoleDto>>> AssignPermissionToRole(
        Guid id,
        [FromBody] AssignPermissionToRoleCommand command)
    {
        command.RoleId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a role by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the role to delete.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a confirmation message if the operation was successful; otherwise, an error response.
    /// </returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse<string>>> DeleteRole(Guid id)
        => Ok(await _mediator.Send(new DeleteRoleCommand { Id = id }));
}
