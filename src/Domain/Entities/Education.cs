#nullable enable
using System;
using NodaTime;

namespace Domain.Entities;

/// <summary>
/// Represents an educational qualification recorded for a user.
/// </summary>
public class Education : BaseEntity<Guid>
{
    /// <summary>
    /// Gets or sets the identifier of the user who owns this education record.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the name of the educational institution.
    /// </summary>
    public string Institution { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the degree or qualification obtained.
    /// </summary>
    public string? Degree { get; set; }

    /// <summary>
    /// Gets or sets the field of study or major.
    /// </summary>
    public string? FieldOfStudy { get; set; }

    /// <summary>
    /// Gets or sets the date on which the user started attending the institution.
    /// </summary>
    public LocalDate StartDate { get; set; }

    /// <summary>
    /// Gets or sets the date on which the user finished. Null indicates the period is ongoing.
    /// </summary>
    public LocalDate? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the grade or GPA achieved.
    /// </summary>
    public string? Grade { get; set; }

    /// <summary>
    /// Gets or sets an optional description of activities, achievements, or coursework.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the user navigation property.
    /// </summary>
    public User? User { get; set; }
}
