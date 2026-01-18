#nullable enable
using Application.Common.Models;
using Application.UserPreferences.Commands;
using Application.UserPreferences.Models;
using Application.UserPreferences.Queries;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// API controller for managing user preferences.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UserPreferencesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserPreferencesController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    public UserPreferencesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a paginated list of user preferences based on the specified query parameters.
    /// </summary>
    /// <param name="query">The query parameters for filtering, sorting, and paginating user preferences.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a paginated list of <see cref="UserPreferenceDto"/> objects.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<BaseResponse<PaginatedEnumerable<UserPreferenceDto>>>> GetUserPreferences(
        [FromQuery] GetUserPreferencesQuery query)
        => Ok(await _mediator.Send(query));

    /// <summary>
    /// Retrieves a paginated list of the current user's preferences.
    /// </summary>
    /// <param name="query">The query parameters for filtering, sorting, and paginating user preferences.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a paginated list of <see cref="UserPreferenceDto"/> objects.
    /// </returns>
    [HttpGet("me")]
    public async Task<ActionResult<BaseResponse<PaginatedEnumerable<UserPreferenceDto>>>> GetMyUserPreferences(
        [FromQuery] GetMyUserPreferencesQuery query)
        => Ok(await _mediator.Send(query));

    /// <summary>
    /// Retrieves a user preference by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user preference to retrieve.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the <see cref="UserPreferenceDto"/> if found; otherwise, an error response.
    /// </returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<UserPreferenceDto>>> GetUserPreferenceById(Guid id)
        => Ok(await _mediator.Send(new GetUserPreferenceByIdQuery { Id = id }));

    /// <summary>
    /// Creates a new user preference with the specified details.
    /// </summary>
    /// <param name="command">The command containing the user preference details to create.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the created <see cref="UserPreferenceDto"/> object.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<BaseResponse<UserPreferenceDto>>> CreateUserPreference(
        [FromBody] CreateUserPreferenceCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUserPreferenceById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Updates an existing user preference with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier of the user preference to update.</param>
    /// <param name="command">The command containing the updated user preference details.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the updated <see cref="UserPreferenceDto"/> object.
    /// </returns>
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<BaseResponse<UserPreferenceDto>>> UpdateUserPreference(
        Guid id,
        [FromBody] UpdateUserPreferenceCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a user preference by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user preference to delete.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a confirmation message if the operation was successful; otherwise, an error response.
    /// </returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse<string>>> DeleteUserPreference(Guid id)
        => Ok(await _mediator.Send(new DeleteUserPreferenceCommand { Id = id }));
}
