#nullable enable
using Application.Common.Models;
using Application.ContactMethods.Commands;
using Application.ContactMethods.Models;
using Application.ContactMethods.Queries;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// API controller for managing contact methods.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ContactMethodsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContactMethodsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    public ContactMethodsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a paginated list of contact methods based on the specified query parameters.
    /// </summary>
    /// <param name="query">The query parameters for filtering, sorting, and paginating contact methods.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a paginated list of <see cref="ContactMethodDto"/> objects.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<BaseResponse<PaginatedEnumerable<ContactMethodDto>>>> GetContactMethods(
        [FromQuery] GetContactMethodsQuery query)
        => Ok(await _mediator.Send(query));

    /// <summary>
    /// Retrieves a contact method by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the contact method to retrieve.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the <see cref="ContactMethodDto"/> if found; otherwise, an error response.
    /// </returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<ContactMethodDto>>> GetContactMethodById(Guid id)
        => Ok(await _mediator.Send(new GetContactMethodByIdQuery { Id = id }));

    /// <summary>
    /// Creates a new contact method with the specified details.
    /// </summary>
    /// <param name="command">The command containing the contact method details to create.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the created <see cref="ContactMethodDto"/> object.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<BaseResponse<ContactMethodDto>>> CreateContactMethod(
        [FromBody] CreateContactMethodCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetContactMethodById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Updates an existing contact method with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier of the contact method to update.</param>
    /// <param name="command">The command containing the updated contact method details.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the updated <see cref="ContactMethodDto"/> object.
    /// </returns>
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<BaseResponse<ContactMethodDto>>> UpdateContactMethod(
        Guid id,
        [FromBody] UpdateContactMethodCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a contact method by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the contact method to delete.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a confirmation message if the operation was successful; otherwise, an error response.
    /// </returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse<string>>> DeleteContactMethod(Guid id)
        => Ok(await _mediator.Send(new DeleteContactMethodCommand { Id = id }));
}
