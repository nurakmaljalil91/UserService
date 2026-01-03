namespace Domain.Common;

/// <summary>
/// Represents the base entity with a unique identifier.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public long Id { get; set; }
}
