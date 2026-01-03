#nullable enable
using NodaTime;
using NodaTime.Text;

namespace Application.Common.Interfaces;

/// <summary>
/// Provides an abstraction for retrieving the current time and date, time zone information,
/// and parsing date and time strings using NodaTime types.
/// </summary>
public interface IClockService
{
    /// <summary>
    /// Gets the time zone used by the clock service.
    /// </summary>
    DateTimeZone TimeZone { get; }

    /// <summary>
    /// Gets the current instant in time.
    /// </summary>
    Instant Now { get; }

    /// <summary>
    /// Gets the current local date and time in the configured time zone.
    /// </summary>
    LocalDateTime LocalNow { get; }
    
    /// <summary>
    /// Gets the current local date in the configured time zone.
    /// </summary>
    LocalDate Today { get; }

    /// <summary>
    /// Converts a local date and time to an instant in time.
    /// </summary>
    /// <param name="local">The local date and time to convert.</param>
    /// <returns>The corresponding instant in time.</returns>
    Instant ToInstant(LocalDateTime local);

    /// <summary>
    /// Converts an instant in time to a local date and time in the configured time zone.
    /// </summary>
    /// <param name="instant">The instant to convert.</param>
    /// <returns>The corresponding local date and time.</returns>
    LocalDateTime ToLocal(Instant instant);

    /// <summary>
    /// Attempts to parse a string as a local date.
    /// </summary>
    /// <param name="date">The date string to parse.</param>
    /// <returns>A parse result containing the parsed <see cref="LocalDate"/> if successful; otherwise, an error.</returns>
    ParseResult<LocalDate>? TryParseDate(string? date);
    
    /// <summary>
    /// Attempts to parse a string as a local date and time.
    /// </summary>
    /// <param name="time">The date and time string to parse.</param>
    /// <returns>A parse result containing the parsed <see cref="LocalDateTime"/> if successful; otherwise, an error.</returns>
    ParseResult<LocalDateTime>? TryParseDateTime(string? time);
}
