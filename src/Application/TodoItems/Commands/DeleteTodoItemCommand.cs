using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Mediator;

namespace Application.TodoItems.Commands;

/// <summary>
/// Represents a command to delete a todos item.
/// </summary>
public class DeleteTodoItemCommand : IRequest<BaseResponse<object>>
{
    public long Id { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteTodoItemCommand"/> class with the specified todos item ID.
    /// </summary>
    /// <param name="id">The ID of the todos item to delete.</param>
    public DeleteTodoItemCommand(long id)
    {
        Id = id;
    }
}

/// <summary>
/// Handles the <see cref="DeleteTodoItemCommand"/> to delete a todos item.
/// </summary>
public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand, BaseResponse<object>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteTodoItemCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public DeleteTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the delete command for a todos item.
    /// </summary>
    /// <param name="request">The delete todos item command.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A response indicating the result of the delete operation.</returns>
    public async Task<BaseResponse<object>> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException("Todo item not found.");
        }
        _context.TodoItems.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return BaseResponse<object>.Ok(null, "Todo item deleted successfully.");
    }
}
