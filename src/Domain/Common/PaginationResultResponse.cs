#nullable enable
namespace Domain.Common;

/// <summary>
/// Represents a paginated response containing data, total count, and current page.
/// </summary>
/// <typeparam name="T">The type of the data contained in the response.</typeparam>
public class PaginationResultResponse<T>
{
    /// <summary>
    /// Gets or sets the paginated data.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Gets or sets the total number of items.
    /// </summary>
    public int Total { get; set; } = 0;

    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    public int Page { get; set; } = 0;
}
