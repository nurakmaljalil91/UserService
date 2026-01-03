using System.Collections.Generic;
using Domain.Common;

namespace Domain.UnitTests.Common;

/// <summary>
/// Unit tests for the <see cref="BaseResponse{T}"/> class.
/// </summary>
public class BaseResponseTests
{
    /// <summary>
    /// Verifies that <see cref="BaseResponse{T}.Ok(T, string?)"/> sets <see cref="BaseResponse{T}.Success"/> to true,
    /// assigns the provided data and message, and leaves <see cref="BaseResponse{T}.Errors"/> as null.
    /// </summary>
    [Fact]
    public void Ok_SetsSuccessDataAndMessage()
    {
        var payload = new { Id = 1 };

        var response = BaseResponse<object>.Ok(payload, "ok");

        Assert.True(response.Success);
        Assert.Equal("ok", response.Message);
        Assert.Same(payload, response.Data);
        Assert.Null(response.Errors);
    }

    /// <summary>
    /// Verifies that <see cref="BaseResponse{T}.Fail(string, System.Collections.Generic.IReadOnlyDictionary{string, string[]})"/>
    /// sets <see cref="BaseResponse{T}.Success"/> to false, assigns the provided message and errors,
    /// and leaves <see cref="BaseResponse{T}.Data"/> as null.
    /// </summary>
    [Fact]
    public void Fail_SetsFailureMessageAndErrors()
    {
        var errors = new Dictionary<string, string[]>
        {
            ["Title"] = new[] { "Required" }
        };

        var response = BaseResponse<object>.Fail("bad", errors);

        Assert.False(response.Success);
        Assert.Equal("bad", response.Message);
        Assert.Same(errors, response.Errors);
        Assert.Null(response.Data);
    }
}
