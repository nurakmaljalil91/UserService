#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.TodoItems.Models;
using Domain.Common;
using FluentValidation;
using Mediator;

namespace Application.TodoItems.Commands;

/// <summary>
/// Command to update a todos item.
/// </summary>
public class UpdateTodoItemComand : IRequest<BaseResponse<TodoItemDto>>
{
    /// <summary>
    /// Gets or sets the identifier of the todos item.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the new title of the todos item.
    /// </summary>
    public string? Title { get; set; }
}

/// <summary>
/// Handles the update command for a todos item.
/// </summary>
public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemComand, BaseResponse<TodoItemDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTodoItemCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UpdateTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the update of a todos item.
    /// </summary>
    /// <param name="request">The update command containing the todos item ID and new title.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="BaseResponse{TodoItemDto}"/> containing the result of the update operation.
    /// </returns>
    public async Task<BaseResponse<TodoItemDto>> Handle(UpdateTodoItemComand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems.FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException("Todo item not found.");
        }

        entity.Title = request.Title ?? entity.Title;

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<TodoItemDto>.Ok(new TodoItemDto(entity), "Todo item updated successfully.");
    }
}

/// <summary>
/// Validates the <see cref="UpdateTodoItemComand"/> command.
/// </summary>
public class UpdateTodoItemCommandValidator : AbstractValidator<UpdateTodoItemComand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTodoItemCommandValidator"/> class.
    /// </summary>
    public UpdateTodoItemCommandValidator()
    {
        RuleFor(m => m.Title)
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters")
            .NotEmpty().WithMessage("Title is required.");
    }
}
