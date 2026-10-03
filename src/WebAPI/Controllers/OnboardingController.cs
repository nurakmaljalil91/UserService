using Application.Authentications.Commands;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// Manages the authenticated user's first-login profile onboarding outcome.
/// </summary>
[ApiController]
[Authorize]
[Route("api/onboarding")]
public sealed class OnboardingController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Initializes the onboarding controller.</summary>
    public OnboardingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Records a completed or skipped onboarding outcome for the current user.</summary>
    [HttpPut("me")]
    public async Task<ActionResult<BaseResponse<string>>> SetOutcome([FromBody] SetOnboardingOutcomeCommand command)
    {
        var response = await _mediator.Send(command);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}
