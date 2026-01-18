#nullable enable
using Domain.Common;
using Mediator;

namespace Application.Groups.Commands;

/// <summary>
/// Command to delete a group by its identifier.
/// </summary>
public class DeleteGroupCommand : IRequest<BaseResponse<string>>
{
    /// <summary>
    /// Gets or sets the group identifier.
    /// </summary>
    public Guid Id { get; set; }
}
