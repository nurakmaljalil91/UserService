#nullable enable
using NodaTime;

namespace Domain.Common;

/// <summary>
/// Represents an auditable entity with creation and update metadata.
/// </summary>
public abstract class BaseAuditableEntity
{
    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public Instant CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last updated.
    /// </summary>
    public Instant UpdatedDate { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last updated the entity.
    /// </summary>
    public string? UpdatedBy { get; set; }
}

