#nullable enable
using Application.Common.Models;
using Application.Groups.Models;
using Domain.Common;
using Mediator;

namespace Application.Groups.Queries;

/// <summary>
/// Represents a paginated request to retrieve groups with optional filtering and sorting.
/// </summary>
public class GetGroupsQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<GroupDto>>>;
