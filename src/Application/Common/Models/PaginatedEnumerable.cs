#nullable enable

namespace Application.Common.Models;

/// <summary>
/// Represents a paginated collection of items with pagination metadata.
/// </summary>
/// <typeparam name="T">The type of the items in the collection.</typeparam>
public class PaginatedEnumerable<T>
{
    /// <summary>
    /// Gets or sets the items for the current page.
    /// </summary>
    public IEnumerable<T>? Items { get; set; }

    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Gets or sets the total number of items.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginatedEnumerable{T}"/> class.
    /// </summary>
    /// <param name="items">The items for the current page.</param>
    /// <param name="count">The total number of items.</param>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    public PaginatedEnumerable(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber <= 0 ? 1 : pageNumber;
        TotalPages = pageSize <= 0 ? 0 : (int)Math.Ceiling(count / (double)pageSize);
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
}
