#nullable enable
using System;
using Application.WorkExperiences.Models;
using Domain.Common;
using Mediator;

namespace Application.WorkExperiences.Commands;

/// <summary>
/// Command to update an existing work experience record.
/// </summary>
public class UpdateWorkExperienceCommand : IRequest<BaseResponse<WorkExperienceDto>>
{
    /// <summary>
    /// Gets or sets the work experience record identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the company or organisation.
    /// </summary>
    public string? Company { get; set; }

    /// <summary>
    /// Gets or sets the job title or position held.
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// Gets or sets the start date in yyyy-MM-dd format.
    /// </summary>
    public string? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date in yyyy-MM-dd format. Null indicates a current position.
    /// </summary>
    public string? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the description of responsibilities and achievements.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the location of the workplace.
    /// </summary>
    public string? Location { get; set; }
}
