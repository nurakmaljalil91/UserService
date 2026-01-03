using Domain.Exceptions;

namespace Domain.UnitTests.Exceptions;

/// <summary>
/// Unit tests for <see cref="Domain.Exceptions.UnsupportedColourException" />.
/// </summary>
public class UnsupportedColourExceptionTests
{
    /// <summary>
    /// Verifies that the exception message includes the provided colour code.
    /// </summary>
    [Fact]
    public void Message_IncludesCode()
    {
        var ex = new UnsupportedColourException("#123456");

        Assert.Equal("Colour \"#123456\" is unsupported.", ex.Message);
    }
}
