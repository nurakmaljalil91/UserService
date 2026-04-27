#nullable enable
using System;
using System.Collections.Generic;
using Application.Skills.Models;
using Domain.Common;
using Mediator;

namespace Application.Skills.Queries;

/// <summary>
/// Query to retrieve all skills for a specific user.
/// </summary>
public class GetSkillsByUserIdQuery : IRequest<BaseResponse<IEnumerable<SkillDto>>>
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }
}
