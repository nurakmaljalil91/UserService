#nullable enable
namespace Domain.Common;

/// <summary>
/// Represents a base class for value objects in Domain-Driven Design (DDD).
/// Value objects are immutable and compared based on their property values.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Determines whether two value objects are equal.
    /// </summary>
    /// <param name="left">The first value object.</param>
    /// <param name="right">The second value object.</param>
    /// <returns><c>true</c> if the value objects are equal; otherwise, <c>false</c>.</returns>
    protected static bool EqualOperator(ValueObject? left, ValueObject? right)
    {
        if (left is null ^ right is null)
        {
            return false;
        }

        return left?.Equals(right!) != false;
    }

    /// <summary>
    /// Determines whether two value objects are not equal.
    /// </summary>
    /// <param name="left">The first value object.</param>
    /// <param name="right">The second value object.</param>
    /// <returns><c>true</c> if the value objects are not equal; otherwise, <c>false</c>.</returns>
    protected static bool NotEqualOperator(ValueObject? left, ValueObject? right)
    {
        return !(EqualOperator(left, right));
    }

    /// <summary>
    /// Gets the components that are used to determine equality for the value object.
    /// </summary>
    /// <returns>An <see cref="System.Collections.Generic.IEnumerable{T}"/> of equality components.</returns>
    protected abstract IEnumerable<object> GetEqualityComponents();

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }
}
