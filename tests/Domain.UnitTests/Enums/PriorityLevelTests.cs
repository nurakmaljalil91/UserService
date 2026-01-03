using Domain.Enums;

namespace Domain.UnitTests.Enums;

/// <summary>
/// Contains unit tests for the <see cref="PriorityLevel"/> enum.
/// </summary>
public class PriorityLevelTests
{
    /// <summary>
    /// Verifies that the <see cref="PriorityLevel"/> enum values are as expected.
    /// </summary>
    [Fact]
    public void Values_AreExpected()
    {
        Assert.Equal(0, (int)PriorityLevel.None);
        Assert.Equal(1, (int)PriorityLevel.Low);
        Assert.Equal(2, (int)PriorityLevel.Medium);
        Assert.Equal(3, (int)PriorityLevel.High);
    }
}
