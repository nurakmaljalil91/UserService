#nullable enable
using Application.Permissions.Models;
using Domain.Common;
using Mediator;

namespace Application.Permissions.Queries;

/// <summary>
/// Query to retrieve a permission by identifier.
/// </summary>
public class GetPermissionByIdQuery : IRequest<BaseResponse<PermissionDto>>
{
    /// <summary>
    /// Gets or sets the permission identifier.
    /// </summary>
    public Guid Id { get; set; }
}
