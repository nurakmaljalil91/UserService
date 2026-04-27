#nullable enable
using System;
using Application.Skills.Models;
using Domain.Common;
using Mediator;

namespace Application.Skills.Commands;

/// <summary>
/// Command to update an existing skill.
/// </summary>
public class UpdateSkillCommand : IRequest<BaseResponse<SkillDto>>
{
    /// <summary>
    /// Gets or sets the skill identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the skill.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the proficiency level.
    /// </summary>
    public string? Proficiency { get; set; }

    /// <summary>
    /// Gets or sets the number of years of experience with this skill.
    /// </summary>
    public int? YearsOfExperience { get; set; }
}
