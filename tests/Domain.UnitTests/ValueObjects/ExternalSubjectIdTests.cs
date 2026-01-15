#nullable enable
using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

/// <summary>
/// Contains unit tests for <see cref="ExternalSubjectId"/>.
/// </summary>
public sealed class ExternalSubjectIdTests
{
    /// <summary>
    /// Tests that the subject identifier value is trimmed.
    /// </summary>
    [Fact]
    public void Constructor_TrimsValue()
    {
        var subject = new ExternalSubjectId(" subject ");

        Assert.Equal("subject", subject.Value);
    }

    /// <summary>
    /// Tests that empty subject identifiers throw an exception.
    /// </summary>
    [Fact]
    public void Constructor_Throws_WhenValueMissing()
    {
        Assert.Throws<ArgumentException>(() => new ExternalSubjectId(""));
    }
}
