#nullable enable
using Application.Common.Interfaces;
using NodaTime.TimeZones;
using NodaTime;
using NodaTime.Text;

namespace Infrastructure.Services;

/// <summary>
/// Provides time-related services using NodaTime, including current instant, local date/time, and parsing utilities.
/// </summary>
public class ClockService : IClockService
{
    private readonly IClock _clock;

    /// <summary>
    /// Gets the time zone used by the clock service.
    /// </summary>
    public DateTimeZone TimeZone { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClockService"/> class using the system clock.
    /// </summary>
    public ClockService()
        : this(SystemClock.Instance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClockService"/> class with a specified clock.
    /// </summary>
    /// <param name="clock">The clock to use for time operations.</param>
    public ClockService(IClock clock)
    {
        _clock = clock;

        // NOTE: Get the current users timezone here instead of hard coding it...
        TimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Asia/Singapore")
            ?? throw new InvalidOperationException("Time zone 'Asia/Singapore' not found in TZDB provider.");
    }

    /// <summary>
    /// Gets the current instant from the underlying clock.
    /// </summary>
    public Instant Now
        => _clock.GetCurrentInstant();

    /// <summary>
    /// Gets the current local date and time in the configured time zone.
    /// </summary>
    public LocalDateTime LocalNow
        => Now.InZone(TimeZone).LocalDateTime;

    /// <summary>
    /// Gets the current local date in the configured time zone.
    /// </summary>
    public LocalDate Today => Now.InZone(TimeZone).Date;

    /// <summary>
    /// Converts a <see cref="LocalDateTime"/> to an <see cref="Instant"/> in the configured time zone.
    /// </summary>
    /// <param name="local">The local date and time to convert.</param>
    /// <returns>The corresponding instant.</returns>
    public Instant ToInstant(LocalDateTime local)
        => local.InZone(TimeZone, Resolvers.LenientResolver).ToInstant();

    /// <summary>
    /// Converts an <see cref="Instant"/> to a <see cref="LocalDateTime"/> in the configured time zone.
    /// </summary>
    /// <param name="instant">The instant to convert.</param>
    /// <returns>The corresponding local date and time.</returns>
    public LocalDateTime ToLocal(Instant instant)
        => instant.InZone(TimeZone).LocalDateTime;

    /// <summary>
    /// Attempts to parse a string as a <see cref="LocalDate"/> using the format "yyyy-MM-dd".
    /// </summary>
    /// <param name="date">The date string to parse.</param>
    /// <returns>The parse result, or null if input is null.</returns>
    public ParseResult<LocalDate>? TryParseDate(string? date)
    {
        var pattern = LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
        return date != null ? pattern.Parse(date) : null;
    }

    /// <summary>
    /// Attempts to parse a string as a <see cref="LocalDateTime"/> using the format "yyyy-MM-ddTHH:mm:ss".
    /// </summary>
    /// <param name="time">The date-time string to parse.</param>
    /// <returns>The parse result, or null if input is null.</returns>
    public ParseResult<LocalDateTime>? TryParseDateTime(string? time)
    {
        var pattern = LocalDateTimePattern.CreateWithInvariantCulture("yyyy-MM-ddTHH:mm:ss");
        return time != null ? pattern.Parse(time) : null;
    }
}

#nullable disable
