#nullable enable
using System;
using System.Globalization;
using Domain.Entities;

namespace Application.WorkExperiences.Models;

/// <summary>
/// Represents a work experience record summary for API responses.
/// </summary>
public sealed record WorkExperienceDto
{
    private const string DateFormat = "yyyy-MM-dd";

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkExperienceDto"/> class.
    /// </summary>
    public WorkExperienceDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkExperienceDto"/> class from a <see cref="WorkExperience"/> entity.
    /// </summary>
    /// <param name="workExperience">The <see cref="WorkExperience"/> entity to map from.</param>
    public WorkExperienceDto(WorkExperience workExperience)
    {
        Id = workExperience.Id;
        UserId = workExperience.UserId;
        Company = workExperience.Company;
        Position = workExperience.Position;
        StartDate = workExperience.StartDate.ToString(DateFormat, CultureInfo.InvariantCulture);
        EndDate = workExperience.EndDate?.ToString(DateFormat, CultureInfo.InvariantCulture);
        Description = workExperience.Description;
        Location = workExperience.Location;
    }

    /// <summary>
    /// Gets the work experience record identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Gets the name of the company or organisation.
    /// </summary>
    public string? Company { get; init; }

    /// <summary>
    /// Gets the job title or position held.
    /// </summary>
    public string? Position { get; init; }

    /// <summary>
    /// Gets the start date in yyyy-MM-dd format.
    /// </summary>
    public string? StartDate { get; init; }

    /// <summary>
    /// Gets the end date in yyyy-MM-dd format. Null indicates a current position.
    /// </summary>
    public string? EndDate { get; init; }

    /// <summary>
    /// Gets the description of responsibilities and achievements.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the location of the workplace.
    /// </summary>
    public string? Location { get; init; }
}
