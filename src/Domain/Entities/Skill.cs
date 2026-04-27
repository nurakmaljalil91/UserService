#nullable enable
using System;

namespace Domain.Entities;

/// <summary>
/// Represents a skill associated with a user.
/// </summary>
public class Skill : BaseEntity<Guid>
{
    /// <summary>
    /// Gets or sets the identifier of the user who owns this skill.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the name of the skill.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the proficiency level (e.g. "Beginner", "Intermediate", "Expert").
    /// </summary>
    public string? Proficiency { get; set; }

    /// <summary>
    /// Gets or sets the number of years of experience with this skill.
    /// </summary>
    public int? YearsOfExperience { get; set; }

    /// <summary>
    /// Gets or sets the user navigation property.
    /// </summary>
    public User? User { get; set; }
}
