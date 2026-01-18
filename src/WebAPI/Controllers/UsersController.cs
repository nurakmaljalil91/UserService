using Application.Common.Models;
using Application.Users.Commands;
using Application.Users.Models;
using Application.Users.Queries;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// API controller for managing users.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a paginated list of users based on the specified query parameters.
    /// </summary>
    /// <param name="query">The query parameters for filtering, sorting, and paginating users.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a paginated list of <see cref="UserDto"/> objects.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<BaseResponse<PaginatedEnumerable<UserDto>>>> GetUsers([FromQuery] GetUsersQuery query)
        => Ok(await _mediator.Send(query));

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the <see cref="UserDto"/> if found; otherwise, an error response.
    /// </returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<UserDto>>> GetUserById(Guid id)
        => Ok(await _mediator.Send(new GetUserByIdQuery { Id = id }));

    /// <summary>
    /// Creates a new user with the specified details.
    /// </summary>
    /// <param name="command">The command containing the user details to create.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the created <see cref="UserDto"/> object.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<BaseResponse<UserDto>>> CreateUser([FromBody] CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUserById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    /// <param name="id">The unique identifier of the user to assign the role to.</param>
    /// <param name="command">The command containing the role assignment details.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the updated <see cref="UserDto"/> object.
    /// </returns>
    [HttpPost("{id:guid}/assign-role")]
    public async Task<ActionResult<BaseResponse<UserDto>>> AssignRoleToUser(Guid id, [FromBody] AssignRoleToUserCommand command)
    {
        command.UserId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing user with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="command">The command containing the updated user details.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the updated <see cref="UserDto"/> object.
    /// </returns>
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<BaseResponse<UserDto>>> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Soft deletes a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to soft delete.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a confirmation message if the operation was successful; otherwise, an error response.
    /// </returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse<string>>> SoftDeleteUser(Guid id)
        => Ok(await _mediator.Send(new SoftDeleteUserCommand { Id = id }));
}
