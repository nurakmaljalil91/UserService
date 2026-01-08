#nullable enable
using Domain.Common;
using Mediator;

namespace Application.Permissions.Commands;

/// <summary>
/// Command to delete a permission by its identifier.
/// </summary>
public class DeletePermissionCommand : IRequest<BaseResponse<string>>
{
    /// <summary>
    /// Gets or sets the permission identifier.
    /// </summary>
    public Guid Id { get; set; }
}
