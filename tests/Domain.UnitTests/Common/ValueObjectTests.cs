#nullable enable
using Domain.Common;

namespace Domain.UnitTests.Common;

/// <summary>
/// Contains unit tests for the <see cref="ValueObject"/> base class and its equality logic.
/// </summary>
public class ValueObjectTests
{
    private sealed class SampleValueObject(int number, string? text) : ValueObject
    {
        private int Number { get; } = number;
        private string? Text { get; } = text;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Number;
            yield return Text!;
        }

        public static bool AreEqual(SampleValueObject? left, SampleValueObject? right)
            => EqualOperator(left, right);

        public static bool AreNotEqual(SampleValueObject? left, SampleValueObject? right)
            => NotEqualOperator(left, right);

        public override bool Equals(object? obj)
            => base.Equals(obj);

        public override int GetHashCode()
            => base.GetHashCode();
    }

    /// <summary>
    /// Verifies that two <see cref="SampleValueObject"/> instances with the same component values are considered equal.
    /// </summary>
    [Fact]
    public void Equals_SameComponents_AreEqual()
    {
        var a = new SampleValueObject(1, "a");
        var b = new SampleValueObject(1, "a");

        Assert.True(a.Equals(b));
        Assert.True(SampleValueObject.AreEqual(a, b));
        Assert.False(SampleValueObject.AreNotEqual(a, b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    /// <summary>
    /// Verifies that two <see cref="SampleValueObject"/> instances with different component values are not considered equal.
    /// </summary>
    [Fact]
    public void Equals_DifferentComponents_AreNotEqual()
    {
        var a = new SampleValueObject(1, "a");
        var b = new SampleValueObject(2, "a");

        Assert.False(a.Equals(b));
        Assert.False(SampleValueObject.AreEqual(a, b));
        Assert.True(SampleValueObject.AreNotEqual(a, b));
    }

    /// <summary>
    /// Verifies correct equality and inequality operator behavior when comparing <see cref="SampleValueObject"/> instances to <c>null</c>.
    /// </summary>
    [Fact]
    public void Equals_NullHandling_Works()
    {
        SampleValueObject? a = null;
        SampleValueObject? b = null;
        var c = new SampleValueObject(1, "a");

        Assert.True(SampleValueObject.AreEqual(a, b));
        Assert.True(SampleValueObject.AreNotEqual(a, c));
    }
}
