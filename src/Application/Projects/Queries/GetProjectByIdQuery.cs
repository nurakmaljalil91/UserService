#nullable enable
using System;
using Application.Projects.Models;
using Domain.Common;
using Mediator;

namespace Application.Projects.Queries;

/// <summary>
/// Query to retrieve a project by identifier.
/// </summary>
public class GetProjectByIdQuery : IRequest<BaseResponse<ProjectDto>>
{
    /// <summary>
    /// Gets or sets the project identifier.
    /// </summary>
    public Guid Id { get; set; }
}
