#nullable enable
namespace Application.Common.Models;

/// <summary>
/// Represents a request for paginated data, including page number, page size, sorting, and filtering options.
/// </summary>
public class PaginatedRequest
{
    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the total number of items per page.
    /// </summary>
    public int Total { get; set; } = 10;

    /// <summary>
    /// Gets or sets the property name to sort by.
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the sorting is descending.
    /// </summary>
    public bool Descending { get; set; }

    /// <summary>
    /// Gets or sets the filter string to apply to the data.
    /// </summary>
    public string? Filter { get; set; }
}
