namespace Application.Common.Exceptions;

/// <summary>
/// Exception that is thrown when an entity is not found in the data store.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class.
    /// </summary>
    public NotFoundException()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class for a specific entity type and key.
    /// </summary>
    /// <param name="name">The name of the entity type.</param>
    /// <param name="key">The key value of the entity.</param>
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}
