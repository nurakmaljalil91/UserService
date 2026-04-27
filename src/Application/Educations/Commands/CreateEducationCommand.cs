#nullable enable
using System;
using Application.Educations.Models;
using Domain.Common;
using Mediator;

namespace Application.Educations.Commands;

/// <summary>
/// Command to create a new education record for a user.
/// </summary>
public class CreateEducationCommand : IRequest<BaseResponse<EducationDto>>
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the name of the educational institution.
    /// </summary>
    public string Institution { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the degree or qualification obtained.
    /// </summary>
    public string? Degree { get; set; }

    /// <summary>
    /// Gets or sets the field of study or major.
    /// </summary>
    public string? FieldOfStudy { get; set; }

    /// <summary>
    /// Gets or sets the start date in yyyy-MM-dd format.
    /// </summary>
    public string StartDate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the end date in yyyy-MM-dd format. Null indicates ongoing.
    /// </summary>
    public string? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the grade or GPA achieved.
    /// </summary>
    public string? Grade { get; set; }

    /// <summary>
    /// Gets or sets the description of activities or coursework.
    /// </summary>
    public string? Description { get; set; }
}
