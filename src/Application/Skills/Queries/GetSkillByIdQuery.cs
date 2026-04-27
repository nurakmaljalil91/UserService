#nullable enable
using System;
using Application.Skills.Models;
using Domain.Common;
using Mediator;

namespace Application.Skills.Queries;

/// <summary>
/// Query to retrieve a skill by identifier.
/// </summary>
public class GetSkillByIdQuery : IRequest<BaseResponse<SkillDto>>
{
    /// <summary>
    /// Gets or sets the skill identifier.
    /// </summary>
    public Guid Id { get; set; }
}
