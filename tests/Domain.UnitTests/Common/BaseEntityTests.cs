#nullable enable
using Domain.Common;

namespace Domain.UnitTests.Common;

/// <summary>
/// Unit tests for the <see cref="BaseEntity{TKey}"/> class.
/// </summary>
public class BaseEntityTests
{
    private sealed class TestEntity : BaseEntity<long>
    {
    }

    /// <summary>
    /// Verifies that the <see cref="BaseEntity{TKey}.Id"/> property can be set and retrieved.
    /// </summary>
    [Fact]
    public void Id_CanBeSet()
    {
        var entity = new TestEntity { Id = 42 };

        Assert.Equal(42, entity.Id);
    }
}
