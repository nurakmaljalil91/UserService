using Application.Common.Models;
using Application.TodoLists.Commands;
using Application.TodoLists.Models;
using Application.TodoLists.Queries;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// API controller for managing todos lists.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class TodoListsController : ControllerBase
{
    private readonly IMediator _mediator;
    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator for handling requests.</param>
    public TodoListsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all todos lists with optional pagination, sorting, and filtering.
    /// </summary>
    /// <param name="query">The query parameters for pagination, sorting, and filtering.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> containing a paginated list of <see cref="TodoListDto"/>.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<BaseResponse<PaginatedEnumerable<TodoListDto>>>> GetAllTodoLists([FromQuery] GetTodoListsQuery query)
    => Ok(await _mediator.Send(query));

    /// <summary>
    /// Creates a new todos list.
    /// </summary>
    /// <param name="command">The command containing the details of the todos list to create.</param>
    /// <returns>
    /// A <see cref="BaseResponse{TodoListDto}"/> containing the created todos list.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<BaseResponse<TodoListDto>>> CreateTodoList([FromBody] CreateTodoListCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAllTodoLists), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Updates an existing todos list.
    /// </summary>
    /// <param name="command">The command containing the updated details of the todos list.</param>
    /// <returns>
    /// A <see cref="BaseResponse{TodoListDto}"/> containing the updated todos list.
    /// </returns>
    [HttpPatch]
    public async Task<ActionResult<BaseResponse<TodoListDto>>> UpdateTodoList([FromBody] UpdateTodoListCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a todos list by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the todos list to delete.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> indicating the result of the delete operation.
    /// </returns>
    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<string>>> DeleteTodoList(long id)
    {
        var command = new DeleteTodoListCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
