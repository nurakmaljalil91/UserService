using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.TodoLists.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.TodoLists.Queries;

/// <summary>
/// Represents a paginated request to retrieve task lists with optional filtering and sorting.
/// </summary>
public class GetTodoListsQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<TodoListDto>>>;

/// <summary>
/// Handles the retrieval of paginated task lists with optional filtering and sorting.
/// </summary>
public class GetTodoListsQueryHandler : IRequestHandler<GetTodoListsQuery, BaseResponse<PaginatedEnumerable<TodoListDto>>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTodoListsQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetTodoListsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<PaginatedEnumerable<TodoListDto>>> Handle(GetTodoListsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TodoLists
            .AsQueryable()
            .ApplyFilters(request.Filter)
            .ApplySorting(request.SortBy, request.Descending);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.Total <= 0 ? 10 : request.Total;

        var totalCount = await query.CountAsync(cancellationToken);

        var result = await query
            .Include(t => t.Items)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(todoList => new TodoListDto(todoList))
            .ToListAsync(cancellationToken);

        var paginatedResult = new PaginatedEnumerable<TodoListDto>(result, totalCount, page, pageSize);

        return BaseResponse<PaginatedEnumerable<TodoListDto>>.Ok(paginatedResult, $"Successfully retrieved {paginatedResult.Items?.Count() ?? 0} todo lists.");

    }
}
