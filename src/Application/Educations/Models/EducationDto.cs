#nullable enable
using System;
using System.Globalization;
using Domain.Entities;

namespace Application.Educations.Models;

/// <summary>
/// Represents an education record summary for API responses.
/// </summary>
public sealed record EducationDto
{
    private const string DateFormat = "yyyy-MM-dd";

    /// <summary>
    /// Initializes a new instance of the <see cref="EducationDto"/> class.
    /// </summary>
    public EducationDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EducationDto"/> class from an <see cref="Education"/> entity.
    /// </summary>
    /// <param name="education">The <see cref="Education"/> entity to map from.</param>
    public EducationDto(Education education)
    {
        Id = education.Id;
        UserId = education.UserId;
        Institution = education.Institution;
        Degree = education.Degree;
        FieldOfStudy = education.FieldOfStudy;
        StartDate = education.StartDate.ToString(DateFormat, CultureInfo.InvariantCulture);
        EndDate = education.EndDate?.ToString(DateFormat, CultureInfo.InvariantCulture);
        Grade = education.Grade;
        Description = education.Description;
    }

    /// <summary>
    /// Gets the education record identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Gets the name of the educational institution.
    /// </summary>
    public string? Institution { get; init; }

    /// <summary>
    /// Gets the degree or qualification obtained.
    /// </summary>
    public string? Degree { get; init; }

    /// <summary>
    /// Gets the field of study or major.
    /// </summary>
    public string? FieldOfStudy { get; init; }

    /// <summary>
    /// Gets the start date in yyyy-MM-dd format.
    /// </summary>
    public string? StartDate { get; init; }

    /// <summary>
    /// Gets the end date in yyyy-MM-dd format. Null indicates ongoing.
    /// </summary>
    public string? EndDate { get; init; }

    /// <summary>
    /// Gets the grade or GPA achieved.
    /// </summary>
    public string? Grade { get; init; }

    /// <summary>
    /// Gets the description of activities or coursework.
    /// </summary>
    public string? Description { get; init; }
}
