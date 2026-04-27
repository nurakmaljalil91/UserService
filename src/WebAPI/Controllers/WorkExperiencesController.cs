#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.WorkExperiences.Commands;
using Application.WorkExperiences.Models;
using Application.WorkExperiences.Queries;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// API controller for managing work experience records.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class WorkExperiencesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkExperiencesController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    public WorkExperiencesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new work experience record.
    /// </summary>
    /// <param name="command">The command containing the work experience details.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing the created <see cref="WorkExperienceDto"/>.</returns>
    [HttpPost]
    public async Task<ActionResult<BaseResponse<WorkExperienceDto>>> CreateWorkExperience([FromBody] CreateWorkExperienceCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetWorkExperienceById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Retrieves all work experience records for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing a list of <see cref="WorkExperienceDto"/> objects.</returns>
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<BaseResponse<IEnumerable<WorkExperienceDto>>>> GetWorkExperiencesByUserId(Guid userId)
        => Ok(await _mediator.Send(new GetWorkExperiencesByUserIdQuery { UserId = userId }));

    /// <summary>
    /// Retrieves a work experience record by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the work experience record.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing the <see cref="WorkExperienceDto"/> if found.</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<WorkExperienceDto>>> GetWorkExperienceById(Guid id)
        => Ok(await _mediator.Send(new GetWorkExperienceByIdQuery { Id = id }));

    /// <summary>
    /// Updates an existing work experience record.
    /// </summary>
    /// <param name="id">The unique identifier of the work experience record to update.</param>
    /// <param name="command">The command containing the updated work experience details.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing the updated <see cref="WorkExperienceDto"/>.</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BaseResponse<WorkExperienceDto>>> UpdateWorkExperience(Guid id, [FromBody] UpdateWorkExperienceCommand command)
    {
        command.Id = id;
        return Ok(await _mediator.Send(command));
    }

    /// <summary>
    /// Deletes a work experience record by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the work experience record to delete.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing a confirmation message.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse<string>>> DeleteWorkExperience(Guid id)
        => Ok(await _mediator.Send(new DeleteWorkExperienceCommand { Id = id }));
}
