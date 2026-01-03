namespace Application.Common.Interfaces;

/// <summary>
/// Provides an abstraction for retrieving the current date and time.
/// </summary>
public interface IDateTime
{
    /// <summary>
    /// Gets the current local date and time.
    /// </summary>
    DateTime Now { get; }

    /// <summary>
    /// Gets the current local date and time with offset information.
    /// </summary>
    DateTimeOffset NowOffset { get; }

    /// <summary>
    /// Gets the current UTC date and time.
    /// </summary>
    DateTime UtcNow { get; }
}
