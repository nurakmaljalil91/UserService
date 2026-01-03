using Domain.Common;

namespace Domain.UnitTests.Common;

/// <summary>
/// Unit tests for <see cref="PaginationResultResponse{T}"/>.
/// </summary>
public class PaginationResultResponseTests
{
    /// <summary>
    /// Verifies that the default values of <see cref="PaginationResultResponse{T}"/> are zero or null.
    /// </summary>
    [Fact]
    public void Defaults_AreZeroOrNull()
    {
        var response = new PaginationResultResponse<string>();

        Assert.Null(response.Data);
        Assert.Equal(0, response.Page);
        Assert.Equal(0, response.Total);
    }

    /// <summary>
    /// Verifies that the properties of <see cref="PaginationResultResponse{T}"/> can be set and retrieved.
    /// </summary>
    [Fact]
    public void Properties_CanBeSet()
    {
        var response = new PaginationResultResponse<string>
        {
            Data = "data",
            Page = 2,
            Total = 50
        };

        Assert.Equal("data", response.Data);
        Assert.Equal(2, response.Page);
        Assert.Equal(50, response.Total);
    }
}
