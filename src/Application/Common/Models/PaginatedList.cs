using Microsoft.EntityFrameworkCore;

namespace Application.Common.Models;

/// <summary>
/// Represents a paginated list of items with pagination metadata.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
public class PaginatedList<T>
{
    /// <summary>
    /// Gets the items for the current page.
    /// </summary>
    public List<T> Items { get; }

    /// <summary>
    /// Gets the current page number.
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages { get; }

    /// <summary>
    /// Gets the total number of items.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginatedList{T}"/> class.
    /// </summary>
    /// <param name="items">The items for the current page.</param>
    /// <param name="count">The total number of items.</param>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Creates a paginated list asynchronously from the specified source.
    /// </summary>
    /// <param name="source">The source queryable.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page. If 0, all items are returned.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the paginated list.</returns>
    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();

        // if pageSize is 0, return all items
        if (pageSize == 0) pageSize = count;

        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}
