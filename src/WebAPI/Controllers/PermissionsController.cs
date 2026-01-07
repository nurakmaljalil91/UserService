#nullable enable
using Application.Common.Models;
using Application.Permissions.Commands;
using Application.Permissions.Models;
using Application.Permissions.Queries;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// API controller for managing permissions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PermissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    public PermissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a paginated list of permissions based on the specified query parameters.
    /// </summary>
    /// <param name="query">The query parameters for filtering, sorting, and paginating permissions.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a paginated list of <see cref="PermissionDto"/> objects.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<BaseResponse<PaginatedEnumerable<PermissionDto>>>> GetPermissions([FromQuery] GetPermissionsQuery query)
        => Ok(await _mediator.Send(query));

    /// <summary>
    /// Retrieves a permission by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the permission to retrieve.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the <see cref="PermissionDto"/> if found; otherwise, an error response.
    /// </returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<PermissionDto>>> GetPermissionById(Guid id)
        => Ok(await _mediator.Send(new GetPermissionByIdQuery { Id = id }));

    /// <summary>
    /// Creates a new permission with the specified details.
    /// </summary>
    /// <param name="command">The command containing the permission details to create.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the created <see cref="PermissionDto"/> object.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<BaseResponse<PermissionDto>>> CreatePermission([FromBody] CreatePermissionCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetPermissionById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Updates an existing permission with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier of the permission to update.</param>
    /// <param name="command">The command containing the updated permission details.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the updated <see cref="PermissionDto"/> object.
    /// </returns>
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<BaseResponse<PermissionDto>>> UpdatePermission(Guid id, [FromBody] UpdatePermissionCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a permission by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the permission to delete.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a confirmation message if the operation was successful; otherwise, an error response.
    /// </returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse<string>>> DeletePermission(Guid id)
        => Ok(await _mediator.Send(new DeletePermissionCommand { Id = id }));
}
