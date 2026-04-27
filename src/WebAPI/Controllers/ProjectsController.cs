#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Projects.Commands;
using Application.Projects.Models;
using Application.Projects.Queries;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// API controller for managing project records.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new project record.
    /// </summary>
    /// <param name="command">The command containing the project details.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing the created <see cref="ProjectDto"/>.</returns>
    [HttpPost]
    public async Task<ActionResult<BaseResponse<ProjectDto>>> CreateProject([FromBody] CreateProjectCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProjectById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Retrieves all projects for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing a list of <see cref="ProjectDto"/> objects.</returns>
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<BaseResponse<IEnumerable<ProjectDto>>>> GetProjectsByUserId(Guid userId)
        => Ok(await _mediator.Send(new GetProjectsByUserIdQuery { UserId = userId }));

    /// <summary>
    /// Retrieves a project by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the project.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing the <see cref="ProjectDto"/> if found.</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<ProjectDto>>> GetProjectById(Guid id)
        => Ok(await _mediator.Send(new GetProjectByIdQuery { Id = id }));

    /// <summary>
    /// Updates an existing project record.
    /// </summary>
    /// <param name="id">The unique identifier of the project to update.</param>
    /// <param name="command">The command containing the updated project details.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing the updated <see cref="ProjectDto"/>.</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BaseResponse<ProjectDto>>> UpdateProject(Guid id, [FromBody] UpdateProjectCommand command)
    {
        command.Id = id;
        return Ok(await _mediator.Send(command));
    }

    /// <summary>
    /// Deletes a project by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the project to delete.</param>
    /// <returns>A <see cref="BaseResponse{T}"/> containing a confirmation message.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse<string>>> DeleteProject(Guid id)
        => Ok(await _mediator.Send(new DeleteProjectCommand { Id = id }));
}
