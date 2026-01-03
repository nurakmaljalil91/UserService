#nullable enable
using Application.Common.Interfaces;
using NodaTime;
using NodaTime.Text;
using NodaTime.TimeZones;

namespace Application.UnitTests.TestInfrastructure;

/// <summary>
/// Provides a fixed clock for unit tests.
/// </summary>
public sealed class TestClockService : IClockService
{
    private readonly Instant _now;

    public TestClockService(Instant? now = null)
    {
        _now = now ?? Instant.FromUtc(2025, 1, 1, 0, 0);
        TimeZone = DateTimeZone.Utc;
    }

    public DateTimeZone TimeZone { get; }

    public Instant Now => _now;

    public LocalDateTime LocalNow => Now.InZone(TimeZone).LocalDateTime;

    public LocalDate Today => Now.InZone(TimeZone).Date;

    public Instant ToInstant(LocalDateTime local)
        => local.InZone(TimeZone, Resolvers.LenientResolver).ToInstant();

    public LocalDateTime ToLocal(Instant instant)
        => instant.InZone(TimeZone).LocalDateTime;

    public ParseResult<LocalDate>? TryParseDate(string? date)
    {
        var pattern = LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
        return date != null ? pattern.Parse(date) : null;
    }

    public ParseResult<LocalDateTime>? TryParseDateTime(string? time)
    {
        var pattern = LocalDateTimePattern.CreateWithInvariantCulture("yyyy-MM-ddTHH:mm:ss");
        return time != null ? pattern.Parse(time) : null;
    }
}
