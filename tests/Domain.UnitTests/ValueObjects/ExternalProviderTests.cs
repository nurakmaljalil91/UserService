#nullable enable
using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

/// <summary>
/// Contains unit tests for <see cref="ExternalProvider"/>.
/// </summary>
public sealed class ExternalProviderTests
{
    /// <summary>
    /// Tests that the provider value is normalized.
    /// </summary>
    [Fact]
    public void Constructor_NormalizesValue()
    {
        var provider = new ExternalProvider(" GoOgLe ");

        Assert.Equal("google", provider.Value);
    }

    /// <summary>
    /// Tests that empty provider values throw an exception.
    /// </summary>
    [Fact]
    public void Constructor_Throws_WhenValueMissing()
    {
        Assert.Throws<ArgumentException>(() => new ExternalProvider(" "));
    }
}
