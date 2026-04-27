#nullable enable
using System;
using System.Collections.Generic;
using Application.WorkExperiences.Models;
using Domain.Common;
using Mediator;

namespace Application.WorkExperiences.Queries;

/// <summary>
/// Query to retrieve all work experience records for a specific user.
/// </summary>
public class GetWorkExperiencesByUserIdQuery : IRequest<BaseResponse<IEnumerable<WorkExperienceDto>>>
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }
}
