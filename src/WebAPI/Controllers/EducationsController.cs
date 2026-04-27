#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Educations.Commands;
using Application.Educations.Models;
using Application.Educations.Queries;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// API controller for managing education records.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class EducationsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="EducationsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    public EducationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new education record.
    /// </summary>
    /// <param name="command">The command containing the education record details.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing the created <see cref="EducationDto"/>.</returns>
    [HttpPost]
    public async Task<ActionResult<BaseResponse<EducationDto>>> CreateEducation([FromBody] CreateEducationCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetEducationById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Retrieves all education records for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing a list of <see cref="EducationDto"/> objects.</returns>
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<BaseResponse<IEnumerable<EducationDto>>>> GetEducationsByUserId(Guid userId)
        => Ok(await _mediator.Send(new GetEducationsByUserIdQuery { UserId = userId }));

    /// <summary>
    /// Retrieves an education record by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the education record.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing the <see cref="EducationDto"/> if found.</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<EducationDto>>> GetEducationById(Guid id)
        => Ok(await _mediator.Send(new GetEducationByIdQuery { Id = id }));

    /// <summary>
    /// Updates an existing education record.
    /// </summary>
    /// <param name="id">The unique identifier of the education record to update.</param>
    /// <param name="command">The command containing the updated education details.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing the updated <see cref="EducationDto"/>.</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BaseResponse<EducationDto>>> UpdateEducation(Guid id, [FromBody] UpdateEducationCommand command)
    {
        command.Id = id;
        return Ok(await _mediator.Send(command));
    }

    /// <summary>
    /// Deletes an education record by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the education record to delete.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing a confirmation message.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse<string>>> DeleteEducation(Guid id)
        => Ok(await _mediator.Send(new DeleteEducationCommand { Id = id }));
}
