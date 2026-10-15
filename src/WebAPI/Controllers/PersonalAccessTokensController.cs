#nullable enable
using Application.PersonalAccessTokens.Commands;
using Application.PersonalAccessTokens.Models;
using Application.PersonalAccessTokens.Queries;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Authorization;

namespace WebAPI.Controllers;

/// <summary>
/// Manages the current user's personal access tokens. Requires an interactive sign-in.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicies.InteractiveOnly)]
public sealed class PersonalAccessTokensController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonalAccessTokensController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator for handling requests.</param>
    public PersonalAccessTokensController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lists the current user's personal access tokens, newest first.
    /// </summary>
    /// <returns>The tokens' metadata (never their secrets).</returns>
    [HttpGet]
    public async Task<ActionResult<BaseResponse<IReadOnlyList<PersonalAccessTokenDto>>>> GetMyTokens()
        => Ok(await _mediator.Send(new GetMyPersonalAccessTokensQuery()));

    /// <summary>
    /// Lists the scopes a personal access token can be granted.
    /// </summary>
    /// <returns>The supported scopes.</returns>
    [HttpGet("scopes")]
    public async Task<ActionResult<BaseResponse<IReadOnlyList<string>>>> GetScopes()
        => Ok(await _mediator.Send(new GetTokenScopesQuery()));

    /// <summary>
    /// Creates a personal access token. The secret is returned only in this response.
    /// </summary>
    /// <param name="command">The token name, scopes and optional expiry.</param>
    /// <returns>The created token including its secret.</returns>
    [HttpPost]
    public async Task<ActionResult<BaseResponse<CreatedPersonalAccessTokenDto>>> CreateToken(
        [FromBody] CreatePersonalAccessTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Revokes one of the current user's personal access tokens.
    /// </summary>
    /// <param name="id">The token identifier.</param>
    /// <returns>The revoked token's metadata.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse<PersonalAccessTokenDto>>> RevokeToken(Guid id)
        => Ok(await _mediator.Send(new RevokePersonalAccessTokenCommand { Id = id }));
}
