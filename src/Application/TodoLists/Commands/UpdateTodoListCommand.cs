#nullable enable

using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.TodoLists.Models;
using Domain.Common;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.TodoLists.Commands;

/// <summary>
/// Command to update a TodoList.
/// </summary>
public class UpdateTodoListCommand : IRequest<BaseResponse<TodoListDto>>
{
    /// <summary>
    /// Gets or sets the identifier of the TodoList to update.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the new title for the TodoList.
    /// </summary>
    public string? Title { get; set; }
}

/// <summary>
/// Handles the update operation for a <see cref="TodoListDto"/>.
/// </summary>
public class UpdateTodoListCommandHandler : IRequestHandler<UpdateTodoListCommand, BaseResponse<TodoListDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTodoListCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UpdateTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the update operation for a <see cref="TodoListDto"/>.
    /// </summary>
    /// <param name="request">The update command containing the TodoList ID and new title.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="BaseResponse{TodoListDto}"/> containing the updated TodoList data if successful,
    /// or an error message if the TodoList was not found.
    /// </returns>
    public async Task<BaseResponse<TodoListDto>> Handle(UpdateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoLists.FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException($"Todo List with id {request.Id} is not found");
        }

        entity.Title = request.Title ?? entity.Title;

        await _context.SaveChangesAsync(cancellationToken);

        var dto = new TodoListDto(entity);

        return BaseResponse<TodoListDto>.Ok(dto, "Todo List updated successfully");
    }
}

/// <summary>
/// Validates the <see cref="UpdateTodoListCommand"/> to ensure the title is provided, does not exceed the maximum length,
/// and is unique among existing TodoLists.
/// </summary>
public class UpdateTodoListCommandValidator : AbstractValidator<UpdateTodoListCommand>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTodoListCommandValidator"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UpdateTodoListCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.")
            .MustAsync(async (title, cancellationToken) =>
            {
                var exists = await _context.TodoLists.AnyAsync(tl => tl.Title == title, cancellationToken);
                return !exists;
            }).WithMessage("A Todo List with the same title already exists.")
            .WithErrorCode("Unique");
    }
}
