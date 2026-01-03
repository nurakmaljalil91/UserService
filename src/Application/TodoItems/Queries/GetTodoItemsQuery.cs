using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.TodoItems.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.TodoItems.Queries;

/// <summary>
/// Represents a paginated query for retrieving todos items.
/// </summary>
public class GetTodoItemsQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<TodoItemDto>>>
{
}

/// <summary>
/// Handles the retrieval of paginated todos items based on the specified query parameters.
/// </summary>
public record GetTodoItemsQueryHandler : IRequestHandler<GetTodoItemsQuery, BaseResponse<PaginatedEnumerable<TodoItemDto>>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTodoItemsQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetTodoItemsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the request to retrieve a paginated list of <see cref="TodoItemDto"/> items based on the specified <paramref name="request"/> parameters.
    /// </summary>
    /// <param name="request">The query parameters for retrieving todos items.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a paginated list of <see cref="TodoItemDto"/>.</returns>
    public async Task<BaseResponse<PaginatedEnumerable<TodoItemDto>>> Handle(GetTodoItemsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TodoItems
            .AsQueryable()
            .ApplyFilters(request.Filter)
            .ApplySorting(request.SortBy, request.Descending);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.Total <= 0 ? 10 : request.Total;

        var totalCount = await query.CountAsync(cancellationToken);

        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(todo => new TodoItemDto(todo))
            .ToListAsync(cancellationToken);

        var paginatedList = new PaginatedEnumerable<TodoItemDto>(result, totalCount, page, pageSize);

        return BaseResponse<PaginatedEnumerable<TodoItemDto>>.Ok(paginatedList, $"Successfully retrieved {paginatedList.Items?.Count() ?? 0} todo items.");
    }
}
