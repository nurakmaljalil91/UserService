#nullable enable
using System;
using Application.Projects.Models;
using Domain.Common;
using Mediator;

namespace Application.Projects.Commands;

/// <summary>
/// Command to create a new project record for a user.
/// </summary>
public class CreateProjectCommand : IRequest<BaseResponse<ProjectDto>>
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the title of the project.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the project.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the start date in yyyy-MM-dd format.
    /// </summary>
    public string? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date in yyyy-MM-dd format. Null indicates the project is ongoing.
    /// </summary>
    public string? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the URL of the project repository or live site.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the comma-separated list of technologies used.
    /// </summary>
    public string? TechStack { get; set; }
}
