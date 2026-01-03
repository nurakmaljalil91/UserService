namespace Domain.Common;

/// <summary>
/// Represents the base entity with a unique identifier and audit metadata.
/// </summary>
/// <typeparam name="TKey">The type of the unique identifier.</typeparam>
public abstract class BaseEntity<TKey> : BaseAuditableEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public TKey Id { get; set; } = default!;
}
