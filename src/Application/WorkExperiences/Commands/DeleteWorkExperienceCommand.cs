#nullable enable
using System;
using Domain.Common;
using Mediator;

namespace Application.WorkExperiences.Commands;

/// <summary>
/// Command to delete a work experience record by its identifier.
/// </summary>
public class DeleteWorkExperienceCommand : IRequest<BaseResponse<string>>
{
    /// <summary>
    /// Gets or sets the work experience record identifier.
    /// </summary>
    public Guid Id { get; set; }
}
