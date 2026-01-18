#nullable enable
using Application.Common.Models;
using Application.UserSessions.Models;
using Application.UserSessions.Queries;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// API controller for retrieving the current user session.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UserSessionController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSessionController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending queries.</param>
    public UserSessionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves the current user's session details.
    /// </summary>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the <see cref="UserSessionDto"/> for the current user.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<BaseResponse<UserSessionDto>>> GetUserSession()
        => Ok(await _mediator.Send(new GetUserSessionQuery()));
}
