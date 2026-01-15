#nullable enable
using NodaTime;
using Domain.ValueObjects;

namespace Domain.Entities;

/// <summary>
/// Represents a linked external account for a user.
/// </summary>
public class ExternalIdentity : BaseEntity<Guid>
{
    /// <summary>
    /// Gets or sets the user identifier that owns this external identity.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the external provider for this identity.
    /// </summary>
    public ExternalProvider Provider { get; set; } = null!;

    /// <summary>
    /// Gets or sets the external subject identifier for the provider.
    /// </summary>
    public ExternalSubjectId SubjectId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the email address associated with the external account.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the display name associated with the external account.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the external identity was linked.
    /// </summary>
    public Instant LinkedAt { get; set; }

    /// <summary>
    /// Gets or sets the user that owns this external identity.
    /// </summary>
    public User? User { get; set; }
}
