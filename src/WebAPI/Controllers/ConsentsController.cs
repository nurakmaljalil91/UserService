#nullable enable
using Application.Common.Models;
using Application.Consents.Commands;
using Application.Consents.Models;
using Application.Consents.Queries;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// API controller for managing consents.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ConsentsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsentsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    public ConsentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a paginated list of consents based on the specified query parameters.
    /// </summary>
    /// <param name="query">The query parameters for filtering, sorting, and paginating consents.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a paginated list of <see cref="ConsentDto"/> objects.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<BaseResponse<PaginatedEnumerable<ConsentDto>>>> GetConsents([FromQuery] GetConsentsQuery query)
        => Ok(await _mediator.Send(query));

    /// <summary>
    /// Retrieves a paginated list of the current user's consents.
    /// </summary>
    /// <param name="query">The query parameters for filtering, sorting, and paginating consents.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a paginated list of <see cref="ConsentDto"/> objects.
    /// </returns>
    [HttpGet("me")]
    public async Task<ActionResult<BaseResponse<PaginatedEnumerable<ConsentDto>>>> GetMyConsents([FromQuery] GetMyConsentsQuery query)
        => Ok(await _mediator.Send(query));

    /// <summary>
    /// Retrieves a consent by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the consent to retrieve.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the <see cref="ConsentDto"/> if found; otherwise, an error response.
    /// </returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<ConsentDto>>> GetConsentById(Guid id)
        => Ok(await _mediator.Send(new GetConsentByIdQuery { Id = id }));

    /// <summary>
    /// Creates a new consent with the specified details.
    /// </summary>
    /// <param name="command">The command containing the consent details to create.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the created <see cref="ConsentDto"/> object.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<BaseResponse<ConsentDto>>> CreateConsent([FromBody] CreateConsentCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetConsentById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Updates an existing consent with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier of the consent to update.</param>
    /// <param name="command">The command containing the updated consent details.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the updated <see cref="ConsentDto"/> object.
    /// </returns>
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<BaseResponse<ConsentDto>>> UpdateConsent(Guid id, [FromBody] UpdateConsentCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a consent by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the consent to delete.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a confirmation message if the operation was successful; otherwise, an error response.
    /// </returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse<string>>> DeleteConsent(Guid id)
        => Ok(await _mediator.Send(new DeleteConsentCommand { Id = id }));
}
