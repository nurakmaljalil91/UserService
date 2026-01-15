using Application.ExternalLinks.Commands;
using Application.ExternalLinks.Models;
using Application.ExternalLinks.Queries;
using Domain.Common;
using Domain.Constants;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// API controller for managing external account links.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ExternalLinksController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalLinksController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    public ExternalLinksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Starts linking a Google account for the current user.
    /// </summary>
    [HttpPost("google/start")]
    public async Task<ActionResult<BaseResponse<ExternalLinkStartResponse>>> StartGoogleLink()
    {
        var result = await _mediator.Send(new StartExternalLinkCommand
        {
            Provider = ExternalProviderNames.Google
        });
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Completes linking a Google account for the current user.
    /// </summary>
    /// <param name="command">The command containing the authorization code and state.</param>
    [HttpPost("google/complete")]
    public async Task<ActionResult<BaseResponse<ExternalLinkDto>>> CompleteGoogleLink(
        [FromBody] CompleteExternalLinkCommand command)
    {
        command.Provider = ExternalProviderNames.Google;
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Unlinks a Google account from the current user.
    /// </summary>
    [HttpDelete("google")]
    public async Task<ActionResult<BaseResponse<string>>> UnlinkGoogle()
    {
        var result = await _mediator.Send(new UnlinkExternalProviderCommand
        {
            Provider = ExternalProviderNames.Google
        });
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Retrieves all external links for the current user.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<BaseResponse<IReadOnlyCollection<ExternalLinkDto>>>> GetExternalLinks(
        [FromQuery] GetExternalLinksQuery query)
    {
        var result = await _mediator.Send(query);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Retrieves a Google Calendar access token for a user, intended for service-to-service access.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    [Authorize(Policy = "PlannerService")]
    [HttpGet("google/calendar-token/{userId:guid}")]
    public async Task<ActionResult<BaseResponse<ExternalAccessTokenDto>>> GetGoogleCalendarToken(Guid userId)
    {
        var result = await _mediator.Send(new GetGoogleCalendarAccessTokenQuery { UserId = userId });
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
