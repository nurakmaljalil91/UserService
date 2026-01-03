using Domain.Common;
using NodaTime;

namespace Domain.UnitTests.Common;

/// <summary>
/// Unit tests for <see cref="BaseAuditableEntity"/> and its properties.
/// </summary>
public class BaseAuditableEntityTests
{
    private sealed class TestAuditableEntity : BaseAuditableEntity
    {
    }

    /// <summary>
    /// Verifies that all properties of <see cref="BaseAuditableEntity"/> can be set and retrieved correctly.
    /// </summary>
    [Fact]
    public void Properties_CanBeSet()
    {
        var created = Instant.FromUtc(2025, 1, 1, 0, 0);
        var updated = Instant.FromUtc(2025, 1, 2, 0, 0);

        var entity = new TestAuditableEntity
        {
            CreatedDate = created,
            CreatedBy = "creator",
            UpdatedDate = updated,
            UpdatedBy = "updater"
        };

        Assert.Equal(created, entity.CreatedDate);
        Assert.Equal("creator", entity.CreatedBy);
        Assert.Equal(updated, entity.UpdatedDate);
        Assert.Equal("updater", entity.UpdatedBy);
    }
}
