#nullable enable
using System;
using System.Globalization;
using Domain.Entities;

namespace Application.Projects.Models;

/// <summary>
/// Represents a project summary for API responses.
/// </summary>
public sealed record ProjectDto
{
    private const string DateFormat = "yyyy-MM-dd";

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectDto"/> class.
    /// </summary>
    public ProjectDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectDto"/> class from a <see cref="Project"/> entity.
    /// </summary>
    /// <param name="project">The <see cref="Project"/> entity to map from.</param>
    public ProjectDto(Project project)
    {
        Id = project.Id;
        UserId = project.UserId;
        Title = project.Title;
        Description = project.Description;
        StartDate = project.StartDate?.ToString(DateFormat, CultureInfo.InvariantCulture);
        EndDate = project.EndDate?.ToString(DateFormat, CultureInfo.InvariantCulture);
        Url = project.Url;
        TechStack = project.TechStack;
    }

    /// <summary>
    /// Gets the project identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Gets the title of the project.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// Gets the description of the project.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the start date in yyyy-MM-dd format.
    /// </summary>
    public string? StartDate { get; init; }

    /// <summary>
    /// Gets the end date in yyyy-MM-dd format. Null indicates the project is ongoing.
    /// </summary>
    public string? EndDate { get; init; }

    /// <summary>
    /// Gets the URL of the project repository or live site.
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// Gets the comma-separated list of technologies used.
    /// </summary>
    public string? TechStack { get; init; }
}
