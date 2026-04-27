#nullable enable
using System;
using NodaTime;

namespace Domain.Entities;

/// <summary>
/// Represents a work experience entry recorded for a user.
/// </summary>
public class WorkExperience : BaseEntity<Guid>
{
    /// <summary>
    /// Gets or sets the identifier of the user who owns this work experience record.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the name of the company or organisation.
    /// </summary>
    public string Company { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the job title or position held.
    /// </summary>
    public string Position { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date on which the user started this role.
    /// </summary>
    public LocalDate StartDate { get; set; }

    /// <summary>
    /// Gets or sets the date on which the user left the role. Null indicates a current position.
    /// </summary>
    public LocalDate? EndDate { get; set; }

    /// <summary>
    /// Gets or sets an optional description of responsibilities and achievements.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the location of the workplace.
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Gets or sets the user navigation property.
    /// </summary>
    public User? User { get; set; }
}
