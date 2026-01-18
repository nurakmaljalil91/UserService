#nullable enable
using Application.Common.Models;
using Application.Sessions.Commands;
using Application.Sessions.Models;
using Application.Sessions.Queries;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// API controller for managing sessions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class SessionsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    public SessionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a paginated list of sessions based on the specified query parameters.
    /// </summary>
    /// <param name="query">The query parameters for filtering, sorting, and paginating sessions.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a paginated list of <see cref="SessionDto"/> objects.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<BaseResponse<PaginatedEnumerable<SessionDto>>>> GetSessions([FromQuery] GetSessionsQuery query)
        => Ok(await _mediator.Send(query));

    /// <summary>
    /// Retrieves a session by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the session to retrieve.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the <see cref="SessionDto"/> if found; otherwise, an error response.
    /// </returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<SessionDto>>> GetSessionById(Guid id)
        => Ok(await _mediator.Send(new GetSessionByIdQuery { Id = id }));

    /// <summary>
    /// Creates a new session with the specified details.
    /// </summary>
    /// <param name="command">The command containing the session details to create.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the created <see cref="SessionDto"/> object.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<BaseResponse<SessionDto>>> CreateSession([FromBody] CreateSessionCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetSessionById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Updates an existing session with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier of the session to update.</param>
    /// <param name="command">The command containing the updated session details.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the updated <see cref="SessionDto"/> object.
    /// </returns>
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<BaseResponse<SessionDto>>> UpdateSession(Guid id, [FromBody] UpdateSessionCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a session by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the session to delete.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a confirmation message if the operation was successful; otherwise, an error response.
    /// </returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse<string>>> DeleteSession(Guid id)
        => Ok(await _mediator.Send(new DeleteSessionCommand { Id = id }));
}
