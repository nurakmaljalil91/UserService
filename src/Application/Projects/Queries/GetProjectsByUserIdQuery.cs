#nullable enable
using System;
using System.Collections.Generic;
using Application.Projects.Models;
using Domain.Common;
using Mediator;

namespace Application.Projects.Queries;

/// <summary>
/// Query to retrieve all projects for a specific user.
/// </summary>
public class GetProjectsByUserIdQuery : IRequest<BaseResponse<IEnumerable<ProjectDto>>>
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }
}
