#nullable enable
using Application.Common.Interfaces;

namespace Application.UnitTests.TestInfrastructure;

/// <summary>
/// Provides deterministic date and time values for unit tests.
/// </summary>
public sealed class TestDateTime : IDateTime
{
    private readonly DateTime _utcNow;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestDateTime"/> class.
    /// </summary>
    /// <param name="utcNow">The UTC date and time to return.</param>
    public TestDateTime(DateTime? utcNow = null)
    {
        _utcNow = utcNow ?? new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }

    /// <inheritdoc />
    public DateTime Now => _utcNow.ToLocalTime();

    /// <inheritdoc />
    public DateTimeOffset NowOffset => new(_utcNow);

    /// <inheritdoc />
    public DateTime UtcNow => _utcNow;
}
