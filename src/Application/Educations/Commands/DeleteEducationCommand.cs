#nullable enable
using System;
using Domain.Common;
using Mediator;

namespace Application.Educations.Commands;

/// <summary>
/// Command to delete an education record by its identifier.
/// </summary>
public class DeleteEducationCommand : IRequest<BaseResponse<string>>
{
    /// <summary>
    /// Gets or sets the education record identifier.
    /// </summary>
    public Guid Id { get; set; }
}
