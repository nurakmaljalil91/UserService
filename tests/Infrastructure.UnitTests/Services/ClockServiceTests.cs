using Infrastructure.Services;
using NodaTime;

namespace Infrastructure.UnitTests.Services;

/// <summary>
/// Unit tests for <see cref="ClockService"/>.
/// </summary>
public class ClockServiceTests
{
    private static readonly Instant FixedInstant = Instant.FromUtc(2024, 1, 2, 3, 4, 5);

    /// <summary>
    /// Verifies the clock service defaults to the Asia/Singapore time zone.
    /// </summary>
    [Fact]
    public void Constructor_SetsAsiaSingaporeTimeZone()
    {
        var service = new ClockService(new FixedClock(FixedInstant));

        Assert.Equal("Asia/Singapore", service.TimeZone.Id);
    }

    /// <summary>
    /// Verifies the clock service returns the instant from the underlying clock.
    /// </summary>
    [Fact]
    public void Now_ReturnsClockInstant()
    {
        var service = new ClockService(new FixedClock(FixedInstant));

        Assert.Equal(FixedInstant, service.Now);
    }

    /// <summary>
    /// Verifies local time conversion uses the configured time zone.
    /// </summary>
    [Fact]
    public void LocalNow_UsesConfiguredTimeZone()
    {
        var service = new ClockService(new FixedClock(FixedInstant));

        var expected = FixedInstant.InZone(service.TimeZone).LocalDateTime;

        Assert.Equal(expected, service.LocalNow);
    }

    /// <summary>
    /// Verifies the local date uses the configured time zone.
    /// </summary>
    [Fact]
    public void Today_UsesConfiguredTimeZone()
    {
        var service = new ClockService(new FixedClock(FixedInstant));

        var expected = FixedInstant.InZone(service.TimeZone).Date;

        Assert.Equal(expected, service.Today);
    }

    /// <summary>
    /// Verifies conversions to and from instants use the configured time zone.
    /// </summary>
    [Fact]
    public void ToInstantAndToLocal_AreConsistent()
    {
        var service = new ClockService(new FixedClock(FixedInstant));
        var local = new LocalDateTime(2024, 2, 3, 10, 30, 0);

        var instant = service.ToInstant(local);
        var roundTrip = service.ToLocal(instant);

        Assert.Equal(local, roundTrip);
    }

    /// <summary>
    /// Verifies date parsing returns null when input is null.
    /// </summary>
    [Fact]
    public void TryParseDate_ReturnsNullForNullInput()
    {
        var service = new ClockService(new FixedClock(FixedInstant));

        var result = service.TryParseDate(null);

        Assert.Null(result);
    }

    /// <summary>
    /// Verifies date parsing returns the expected value.
    /// </summary>
    [Fact]
    public void TryParseDate_ReturnsParsedDate()
    {
        var service = new ClockService(new FixedClock(FixedInstant));

        var result = service.TryParseDate("2024-12-25");

        Assert.NotNull(result);
        Assert.Equal(new LocalDate(2024, 12, 25), result.GetValueOrThrow());
    }

    /// <summary>
    /// Verifies date-time parsing returns null when input is null.
    /// </summary>
    [Fact]
    public void TryParseDateTime_ReturnsNullForNullInput()
    {
        var service = new ClockService(new FixedClock(FixedInstant));

        var result = service.TryParseDateTime(null);

        Assert.Null(result);
    }

    /// <summary>
    /// Verifies date-time parsing returns the expected value.
    /// </summary>
    [Fact]
    public void TryParseDateTime_ReturnsParsedDateTime()
    {
        var service = new ClockService(new FixedClock(FixedInstant));

        var result = service.TryParseDateTime("2024-12-25T14:30:00");

        Assert.NotNull(result);
        Assert.Equal(new LocalDateTime(2024, 12, 25, 14, 30, 0), result.GetValueOrThrow());
    }

    private sealed class FixedClock : IClock
    {
        private readonly Instant _instant;

        public FixedClock(Instant instant)
        {
            _instant = instant;
        }

        public Instant GetCurrentInstant() => _instant;
    }
}
