using FluentValidation.Results;

namespace Application.Common.Exceptions;

/// <summary>
/// Represents errors that occur during validation of input data.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a default error message.
    /// </summary>
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with the specified validation failures.
    /// </summary>
    /// <param name="failures">The collection of validation failures.</param>
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                failureGroup => failureGroup.Key,
                failureGroup => failureGroup.Select(failured => failured.ErrorMessage).Distinct().ToArray()
                );
    }

    /// <summary>
    /// Gets a dictionary of validation errors, where the key is the property name and the value is an array of error messages.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
