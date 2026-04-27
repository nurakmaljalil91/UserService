#nullable enable
using System;
using NodaTime;

namespace Domain.Entities;

/// <summary>
/// Represents a project entry recorded for a user.
/// </summary>
public class Project : BaseEntity<Guid>
{
    /// <summary>
    /// Gets or sets the identifier of the user who owns this project.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the title of the project.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional description of the project.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the date on which the project started.
    /// </summary>
    public LocalDate? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the date on which the project ended. Null indicates the project is ongoing.
    /// </summary>
    public LocalDate? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the URL of the project repository or live site.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets a comma-separated list of technologies used in the project.
    /// </summary>
    public string? TechStack { get; set; }

    /// <summary>
    /// Gets or sets the user navigation property.
    /// </summary>
    public User? User { get; set; }
}
