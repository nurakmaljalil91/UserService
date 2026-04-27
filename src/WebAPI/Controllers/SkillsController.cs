#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Skills.Commands;
using Application.Skills.Models;
using Application.Skills.Queries;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// API controller for managing user skills.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class SkillsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    public SkillsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new skill.
    /// </summary>
    /// <param name="command">The command containing the skill details.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing the created <see cref="SkillDto"/>.</returns>
    [HttpPost]
    public async Task<ActionResult<BaseResponse<SkillDto>>> CreateSkill([FromBody] CreateSkillCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetSkillById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Retrieves all skills for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing a list of <see cref="SkillDto"/> objects.</returns>
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<BaseResponse<IEnumerable<SkillDto>>>> GetSkillsByUserId(Guid userId)
        => Ok(await _mediator.Send(new GetSkillsByUserIdQuery { UserId = userId }));

    /// <summary>
    /// Retrieves a skill by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the skill.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing the <see cref="SkillDto"/> if found.</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<SkillDto>>> GetSkillById(Guid id)
        => Ok(await _mediator.Send(new GetSkillByIdQuery { Id = id }));

    /// <summary>
    /// Updates an existing skill.
    /// </summary>
    /// <param name="id">The unique identifier of the skill to update.</param>
    /// <param name="command">The command containing the updated skill details.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing the updated <see cref="SkillDto"/>.</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BaseResponse<SkillDto>>> UpdateSkill(Guid id, [FromBody] UpdateSkillCommand command)
    {
        command.Id = id;
        return Ok(await _mediator.Send(command));
    }

    /// <summary>
    /// Deletes a skill by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the skill to delete.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing a confirmation message.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse<string>>> DeleteSkill(Guid id)
        => Ok(await _mediator.Send(new DeleteSkillCommand { Id = id }));
}
