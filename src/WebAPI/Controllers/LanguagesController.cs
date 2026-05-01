#nullable enable
using System;
using System.Threading.Tasks;
using Application.Languages.Commands;
using Application.Languages.Models;
using Application.Languages.Queries;
using Application.Common.Models;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// API controller for managing user languages.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class LanguagesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguagesController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    public LanguagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a paginated list of languages with optional filtering.
    /// </summary>
    /// <param name="query">The query parameters.</param>
    /// <returns>A paginated list of <see cref="LanguageDto"/> objects.</returns>
    [HttpGet]
    public async Task<ActionResult<BaseResponse<PaginatedEnumerable<LanguageDto>>>> GetLanguages([FromQuery] GetLanguagesQuery query)
        => Ok(await _mediator.Send(query));

    /// <summary>
    /// Retrieves a language by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the language.</param>
    /// <returns>The <see cref="LanguageDto"/> if found.</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<LanguageDto>>> GetLanguageById(Guid id)
        => Ok(await _mediator.Send(new GetLanguageByIdQuery { Id = id }));

    /// <summary>
    /// Creates a new language record.
    /// </summary>
    /// <param name="command">The command containing the language details.</param>
    /// <returns>The created <see cref="LanguageDto"/>.</returns>
    [HttpPost]
    public async Task<ActionResult<BaseResponse<LanguageDto>>> CreateLanguage([FromBody] CreateLanguageCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetLanguageById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Updates an existing language record.
    /// </summary>
    /// <param name="id">The unique identifier of the language to update.</param>
    /// <param name="command">The command containing updated details.</param>
    /// <returns>The updated <see cref="LanguageDto"/>.</returns>
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<BaseResponse<LanguageDto>>> UpdateLanguage(Guid id, [FromBody] UpdateLanguageCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a language record by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the language to delete.</param>
    /// <returns>A confirmation response.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse<string>>> DeleteLanguage(Guid id)
        => Ok(await _mediator.Send(new DeleteLanguageCommand { Id = id }));
}
