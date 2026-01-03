using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Mediator;

namespace Application.TodoLists.Commands;

/// <summary>
/// Command to delete a Todos List by its identifier.
/// </summary>
public class DeleteTodoListCommand : IRequest<BaseResponse<string>>
{
    /// <summary>
    /// Gets or sets the identifier of the Todos List to delete.
    /// </summary>
    public long Id { get; set; }
}

/// <summary>
/// Handles the deletion of a Todos List by its identifier.
/// </summary>
public class DeleteTodoListCommandHandler : IRequestHandler<DeleteTodoListCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteTodoListCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public DeleteTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Handles the request to delete a Todos List by its identifier.
    /// </summary>
    /// <param name="request">The command containing the identifier of the Todos List to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="BaseResponse{T}"/> indicating the result of the delete operation.
    /// </returns>
    public async Task<BaseResponse<string>> Handle(DeleteTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoLists.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException("Todo List not found.");
        }
        _context.TodoLists.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return BaseResponse<string>.Ok($"Todo List with id {request.Id} deleted successfully.");
    }
}
