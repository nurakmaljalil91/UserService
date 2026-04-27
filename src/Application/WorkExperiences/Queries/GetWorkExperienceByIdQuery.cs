#nullable enable
using System;
using Application.WorkExperiences.Models;
using Domain.Common;
using Mediator;

namespace Application.WorkExperiences.Queries;

/// <summary>
/// Query to retrieve a work experience record by identifier.
/// </summary>
public class GetWorkExperienceByIdQuery : IRequest<BaseResponse<WorkExperienceDto>>
{
    /// <summary>
    /// Gets or sets the work experience record identifier.
    /// </summary>
    public Guid Id { get; set; }
}
