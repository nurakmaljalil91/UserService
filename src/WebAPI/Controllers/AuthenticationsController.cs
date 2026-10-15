using Application.Authentications.Commands;
using Application.Authentications.Models;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mediator;

namespace WebAPI.Controllers;

/// <summary>
/// Authentication endpoints for login and registration.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public sealed class AuthenticationsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for handling authentication commands.</param>
    public AuthenticationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<BaseResponse<LoginResponse>>> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<BaseResponse<string>>> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Refreshes an access token using a refresh token.
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<BaseResponse<LoginResponse>>> Refresh([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Exchanges a personal access token for a short-lived access token carrying the token's scopes.
    /// </summary>
    /// <param name="command">The command containing the personal access token.</param>
    /// <returns>The access token, or 401 when the personal access token is invalid, expired or revoked.</returns>
    [HttpPost("token-exchange")]
    public async Task<ActionResult<BaseResponse<TokenExchangeResponse>>> TokenExchange([FromBody] TokenExchangeCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : Unauthorized(result);
    }

    /// <summary>
    /// Resets a user's password.
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<ActionResult<BaseResponse<string>>> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
