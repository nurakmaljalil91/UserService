using Infrastructure.Services;

namespace Infrastructure.UnitTests.Services;

/// <summary>
/// Unit tests for <see cref="DateTimeService"/>.
/// </summary>
public class DateTimeServiceTests
{
    /// <summary>
    /// Ensures <see cref="DateTimeService.Now"/> returns a local time within the call window.
    /// </summary>
    [Fact]
    public void Now_ReturnsCurrentLocalTime()
    {
        var service = new DateTimeService();

        var before = DateTime.Now;
        var value = service.Now;
        var after = DateTime.Now;

        Assert.InRange(value, before, after);
    }

    /// <summary>
    /// Ensures <see cref="DateTimeService.NowOffset"/> returns a local time with offset within the call window.
    /// </summary>
    [Fact]
    public void NowOffset_ReturnsCurrentLocalTimeWithOffset()
    {
        var service = new DateTimeService();

        var before = DateTimeOffset.Now;
        var value = service.NowOffset;
        var after = DateTimeOffset.Now;

        Assert.InRange(value, before, after);
    }

    /// <summary>
    /// Ensures <see cref="DateTimeService.UtcNow"/> returns a UTC time within the call window.
    /// </summary>
    [Fact]
    public void UtcNow_ReturnsCurrentUtcTime()
    {
        var service = new DateTimeService();

        var before = DateTime.UtcNow;
        var value = service.UtcNow;
        var after = DateTime.UtcNow;

        Assert.InRange(value, before, after);
    }
}
