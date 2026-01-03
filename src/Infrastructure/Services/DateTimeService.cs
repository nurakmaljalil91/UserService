using System.Globalization;
using Application.Common.Interfaces;

namespace Infrastructure.Services;

/// <summary>
/// Provides the current date and time information.
/// </summary>
public class DateTimeService : IDateTime
{
    /// <summary>
    /// Gets the current local date and time.
    /// </summary>
    public DateTime Now => DateTime.Now;

    /// <summary>
    /// Gets the current local date and time with offset.
    /// </summary>
    public DateTimeOffset NowOffset => DateTimeOffset.Now;

    /// <summary>
    /// Gets the current UTC date and time.
    /// </summary>
    public DateTime UtcNow => DateTime.UtcNow;
}
