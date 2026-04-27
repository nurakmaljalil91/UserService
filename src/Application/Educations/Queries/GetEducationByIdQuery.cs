#nullable enable
using System;
using Application.Educations.Models;
using Domain.Common;
using Mediator;

namespace Application.Educations.Queries;

/// <summary>
/// Query to retrieve an education record by identifier.
/// </summary>
public class GetEducationByIdQuery : IRequest<BaseResponse<EducationDto>>
{
    /// <summary>
    /// Gets or sets the education record identifier.
    /// </summary>
    public Guid Id { get; set; }
}
