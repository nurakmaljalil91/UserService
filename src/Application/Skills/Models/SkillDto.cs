#nullable enable
using System;
using Domain.Entities;

namespace Application.Skills.Models;

/// <summary>
/// Represents a skill summary for API responses.
/// </summary>
public sealed record SkillDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SkillDto"/> class.
    /// </summary>
    public SkillDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillDto"/> class from a <see cref="Skill"/> entity.
    /// </summary>
    /// <param name="skill">The <see cref="Skill"/> entity to map from.</param>
    public SkillDto(Skill skill)
    {
        Id = skill.Id;
        UserId = skill.UserId;
        Name = skill.Name;
        Proficiency = skill.Proficiency;
        YearsOfExperience = skill.YearsOfExperience;
    }

    /// <summary>
    /// Gets the skill identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Gets the name of the skill.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the proficiency level.
    /// </summary>
    public string? Proficiency { get; init; }

    /// <summary>
    /// Gets the number of years of experience.
    /// </summary>
    public int? YearsOfExperience { get; init; }
}
