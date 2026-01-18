#nullable enable
using Application.Common.Models;
using Application.Groups.Models;
using Domain.Common;
using Mediator;

namespace Application.Groups.Queries;

/// <summary>
/// Represents a paginated request to retrieve the current user's groups.
/// </summary>
public class GetMyGroupsQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<GroupDto>>>
{
}
