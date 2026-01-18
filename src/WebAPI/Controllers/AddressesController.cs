#nullable enable
using Application.Addresses.Commands;
using Application.Addresses.Models;
using Application.Addresses.Queries;
using Application.Common.Models;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// API controller for managing addresses.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class AddressesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddressesController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    public AddressesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a paginated list of addresses based on the specified query parameters.
    /// </summary>
    /// <param name="query">The query parameters for filtering, sorting, and paginating addresses.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a paginated list of <see cref="AddressDto"/> objects.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<BaseResponse<PaginatedEnumerable<AddressDto>>>> GetAddresses([FromQuery] GetAddressesQuery query)
        => Ok(await _mediator.Send(query));

    /// <summary>
    /// Retrieves an address by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the address to retrieve.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the <see cref="AddressDto"/> if found; otherwise, an error response.
    /// </returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<AddressDto>>> GetAddressById(Guid id)
        => Ok(await _mediator.Send(new GetAddressByIdQuery { Id = id }));

    /// <summary>
    /// Creates a new address with the specified details.
    /// </summary>
    /// <param name="command">The command containing the address details to create.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the created <see cref="AddressDto"/> object.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<BaseResponse<AddressDto>>> CreateAddress([FromBody] CreateAddressCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAddressById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Updates an existing address with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier of the address to update.</param>
    /// <param name="command">The command containing the updated address details.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing the updated <see cref="AddressDto"/> object.
    /// </returns>
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<BaseResponse<AddressDto>>> UpdateAddress(Guid id, [FromBody] UpdateAddressCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes an address by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the address to delete.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a confirmation message if the operation was successful; otherwise, an error response.
    /// </returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse<string>>> DeleteAddress(Guid id)
        => Ok(await _mediator.Send(new DeleteAddressCommand { Id = id }));
}
