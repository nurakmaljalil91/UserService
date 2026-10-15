#nullable enable
using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

/// <summary>
/// Unit tests for <see cref="TokenScope"/>.
/// </summary>
public class TokenScopeTests
{
    /// <summary>
    /// Verifies that every service has a read and a write scope.
    /// </summary>
    [Fact]
    public void All_ContainsReadAndWriteForEachService()
    {
        Assert.Equal(TokenScope.Services.Count * 2, TokenScope.All.Count);
        Assert.Contains("planner:read", TokenScope.All);
        Assert.Contains("habits:write", TokenScope.All);
    }

    /// <summary>
    /// Verifies that scope values are trimmed and lower-cased.
    /// </summary>
    [Fact]
    public void Constructor_NormalizesValue()
    {
        Assert.Equal("planner:write", new TokenScope("  Planner:WRITE ").Value);
    }

    /// <summary>
    /// Verifies that unknown scopes are rejected.
    /// </summary>
    [Theory]
    [InlineData("planner:admin")]
    [InlineData("billing:read")]
    [InlineData("")]
    [InlineData("planner")]
    public void Constructor_RejectsUnsupportedScope(string value)
    {
        Assert.Throws<ArgumentException>(() => new TokenScope(value));
        Assert.False(TokenScope.IsValid(value));
    }

    /// <summary>
    /// Verifies that write scopes imply read scopes and the result is de-duplicated in canonical order.
    /// </summary>
    [Fact]
    public void Expand_AddsReadForWriteAndDeduplicates()
    {
        var expanded = TokenScope.Expand(["habits:write", "planner:write", "planner:read", "PLANNER:write"]);

        Assert.Equal(new[] { "planner:read", "planner:write", "habits:read", "habits:write" }, expanded);
    }

    /// <summary>
    /// Verifies that expanding an unknown scope fails.
    /// </summary>
    [Fact]
    public void Expand_RejectsUnsupportedScope()
    {
        Assert.Throws<ArgumentException>(() => TokenScope.Expand(["planner:read", "nope:read"]));
    }

    /// <summary>
    /// Verifies value equality.
    /// </summary>
    [Fact]
    public void Equality_IsByValue()
    {
        Assert.Equal(new TokenScope("files:read"), new TokenScope("FILES:read"));
    }
}
