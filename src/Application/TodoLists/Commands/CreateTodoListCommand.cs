#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Interfaces;
using Application.TodoLists.Models;
using Domain.Common;
using Domain.Entities;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.TodoLists.Commands;

/// <summary>
/// Command to create a new Todos List.
/// </summary>
public class CreateTodoListCommand : IRequest<BaseResponse<TodoListDto>>
{
    /// <summary>
    /// Gets or sets the title of the Todos List.
    /// </summary>
    public string? Title { get; set; }
}

/// <summary>
/// Handles the creation of a new Todos List.
/// </summary>
public class CreateTodoListCommandHandler : IRequestHandler<CreateTodoListCommand, BaseResponse<TodoListDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTodoListCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public CreateTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the creation of a new Todos List.
    /// </summary>
    /// <param name="request">The command containing the details for the new Todos List.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A response containing the created Todos List DTO.</returns>
    public async Task<BaseResponse<TodoListDto>> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoList();

        entity.Title = request.Title;

        _context.TodoLists.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        var dto = new TodoListDto(entity);

        return BaseResponse<TodoListDto>.Ok(dto, $"Created Todo List with id {dto.Id}");
    }
}

/// <summary>
/// Validates the <see cref="CreateTodoListCommand"/> to ensure the title is provided, does not exceed the maximum length,
/// and is unique within the Todos Lists.
/// </summary>
public class CreateTodoListCommandValidator : FluentValidation.AbstractValidator<CreateTodoListCommand>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTodoListCommandValidator"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public CreateTodoListCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
            .MustAsync(async (title, cancellationToken) =>
            {
                var exists = await _context.TodoLists
                    .AnyAsync(tl => tl.Title == title, cancellationToken);
                return !exists;
            }).WithMessage("A Todo List with the same title already exists.")
            .WithErrorCode("Unique");
    }
}
