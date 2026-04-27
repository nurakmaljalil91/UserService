#nullable enable
using System;
using System.Collections.Generic;
using Application.Educations.Models;
using Domain.Common;
using Mediator;

namespace Application.Educations.Queries;

/// <summary>
/// Query to retrieve all education records for a specific user.
/// </summary>
public class GetEducationsByUserIdQuery : IRequest<BaseResponse<IEnumerable<EducationDto>>>
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }
}
