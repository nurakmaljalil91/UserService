#nullable enable
using System;
using Domain.Common;
using Mediator;

namespace Application.Skills.Commands;

/// <summary>
/// Command to delete a skill by its identifier.
/// </summary>
public class DeleteSkillCommand : IRequest<BaseResponse<string>>
{
    /// <summary>
    /// Gets or sets the skill identifier.
    /// </summary>
    public Guid Id { get; set; }
}
