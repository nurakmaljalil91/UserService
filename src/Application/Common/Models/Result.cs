namespace Application.Common.Models;

/// <summary>
/// Represents the result of an operation, indicating success or failure and containing error messages if any.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="succeeded">Indicates whether the operation succeeded.</param>
    /// <param name="errors">A collection of error messages.</param>
    internal Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    /// <summary>
    /// Gets or sets a value indicating whether the operation succeeded.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Gets or sets the error messages associated with the result.
    /// </summary>
    public string[] Errors { get; set; }

    /// <summary>
    /// Creates a successful <see cref="Result"/>.
    /// </summary>
    /// <returns>A <see cref="Result"/> indicating success.</returns>
    public static Result Success()
    {
        return new Result(true, Array.Empty<string>());
    }

    /// <summary>
    /// Creates a failed <see cref="Result"/> with the specified errors.
    /// </summary>
    /// <param name="errors">A collection of error messages.</param>
    /// <returns>A <see cref="Result"/> indicating failure.</returns>
    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result(false, errors);
    }
}
