#nullable enable
using System;
using Application.Skills.Models;
using Domain.Common;
using Mediator;

namespace Application.Skills.Commands;

/// <summary>
/// Command to create a new skill for a user.
/// </summary>
public class CreateSkillCommand : IRequest<BaseResponse<SkillDto>>
{
    /// <summary>
    /// Gets or sets the user identifier.
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
}
