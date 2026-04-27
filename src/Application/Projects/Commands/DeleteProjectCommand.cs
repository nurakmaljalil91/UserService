#nullable enable
using System;
using Domain.Common;
using Mediator;

namespace Application.Projects.Commands;

/// <summary>
/// Command to delete a project by its identifier.
/// </summary>
public class DeleteProjectCommand : IRequest<BaseResponse<string>>
{
    /// <summary>
    /// Gets or sets the project identifier.
    /// </summary>
    public Guid Id { get; set; }
}
