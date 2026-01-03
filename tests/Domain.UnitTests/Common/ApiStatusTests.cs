using Domain.Common;

namespace Domain.UnitTests.Common;

/// <summary>
/// Contains unit tests for the <see cref="ApiStatus"/> class.
/// </summary>
public class ApiStatusTests
{
    /// <summary>
    /// Verifies that the properties of <see cref="ApiStatus"/> can be set and retrieved.
    /// </summary>
    [Fact]
    public void Properties_CanBeSet()
    {
        var status = new ApiStatus
        {
            Status = "OK",
            BuildVersion = "1.0.0"
        };

        Assert.Equal("OK", status.Status);
        Assert.Equal("1.0.0", status.BuildVersion);
    }
}
