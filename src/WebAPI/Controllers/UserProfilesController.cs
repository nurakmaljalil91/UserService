#nullable enable
using Application.Common.Models;
using Application.UserProfiles.Commands;
using Application.UserProfiles.Models;
using Application.UserProfiles.Queries;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// API controller for managing user profiles.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UserProfilesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserProfilesController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    public UserProfilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a paginated list of user profiles based on the specified query parameters.
    /// </summary>
    /// <param name="query">The query parameters for filtering, sorting, and paginating user profiles.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a paginated list of <see cref="UserProfileDto"/> objects.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<BaseResponse<PaginatedEnumerable<UserProfileDto>>>> GetUserProfiles([FromQuery] GetUserProfilesQuery query)
        => Ok(await _mediator.Send(query));

    /// <summary>
    /// Retrieves a user profile by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user profile to retrieve.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the <see cref="UserProfileDto"/> if found; otherwise, an error response.
    /// </returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<UserProfileDto>>> GetUserProfileById(Guid id)
        => Ok(await _mediator.Send(new GetUserProfileByIdQuery { Id = id }));

    /// <summary>
    /// Retrieves the profile for the currently authenticated user.
    /// </summary>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the <see cref="UserProfileDto"/> for the current user.
    /// </returns>
    [HttpGet("me")]
    public async Task<ActionResult<BaseResponse<UserProfileDto>>> GetMyUserProfile()
        => Ok(await _mediator.Send(new GetMyUserProfileQuery()));

    /// <summary>
    /// Creates a new user profile with the specified details.
    /// </summary>
    /// <param name="command">The command containing the user profile details to create.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the created <see cref="UserProfileDto"/> object.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<BaseResponse<UserProfileDto>>> CreateUserProfile([FromBody] CreateUserProfileCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUserProfileById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Updates an existing user profile with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier of the user profile to update.</param>
    /// <param name="command">The command containing the updated user profile details.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the updated <see cref="UserProfileDto"/> object.
    /// </returns>
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<BaseResponse<UserProfileDto>>> UpdateUserProfile(Guid id, [FromBody] UpdateUserProfileCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a user profile by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user profile to delete.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a confirmation message if the operation was successful; otherwise, an error response.
    /// </returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse<string>>> DeleteUserProfile(Guid id)
        => Ok(await _mediator.Send(new DeleteUserProfileCommand { Id = id }));
}
