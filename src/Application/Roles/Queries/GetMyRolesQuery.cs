#nullable enable
using Application.Common.Models;
using Application.Roles.Models;
using Domain.Common;
using Mediator;

namespace Application.Roles.Queries;

/// <summary>
/// Represents a paginated request to retrieve the current user's roles.
/// </summary>
public class GetMyRolesQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<RoleDto>>>
{
}
