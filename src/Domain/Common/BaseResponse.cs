#nullable enable
namespace Domain.Common;

/// <summary>
/// Represents a standard response wrapper for service or API operations.
/// Contains success status, message, data payload, and optional error details.
/// </summary>
/// <typeparam name="T">The type of the data payload.</typeparam>
public sealed class BaseResponse<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets an optional message describing the result.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Gets the data payload of the response.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Validation or business errors grouped by field/property name.
    /// </summary>
    public IReadOnlyDictionary<string, string[]>? Errors { get; init; }

    /// <summary>
    /// Creates a successful response with the specified data and optional message.
    /// </summary>
    /// <param name="data">The data payload.</param>
    /// <param name="message">An optional message.</param>
    /// <returns>A successful <see cref="BaseResponse{T}"/> instance.</returns>
    public static BaseResponse<T> Ok(T data, string? message = null)
        => new()
        {
            Success = true,
            Data = data,
            Message = message
        };

    /// <summary>
    /// Creates a failed response with the specified message and optional errors.
    /// </summary>
    /// <param name="message">The failure message.</param>
    /// <param name="errors">An optional list of error messages.</param>
    /// <returns>A failed <see cref="BaseResponse{T}"/> instance.</returns>
    public static BaseResponse<T> Fail(
        string message,
        IReadOnlyDictionary<string, string[]>? errors = null)
        => new()
        {
            Success = false,
            Message = message,
            Errors = errors
        };
}

