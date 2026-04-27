#nullable enable
using System;
using Application.Educations.Models;
using Domain.Common;
using Mediator;

namespace Application.Educations.Commands;

/// <summary>
/// Command to update an existing education record.
/// </summary>
public class UpdateEducationCommand : IRequest<BaseResponse<EducationDto>>
{
    /// <summary>
    /// Gets or sets the education record identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the educational institution.
    /// </summary>
    public string? Institution { get; set; }

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
    public string? StartDate { get; set; }

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
