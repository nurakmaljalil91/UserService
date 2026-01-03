namespace Application.Common.Exceptions;

/// <summary>
/// Exception that is thrown when an operation is attempted without sufficient permissions.
/// </summary>
public class ForbiddenAccessException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenAccessException"/> class.
    /// </summary>
    public ForbiddenAccessException() : base("Forbidden.") { }
}
