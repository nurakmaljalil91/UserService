#nullable enable
using Application.Common.Models;
using Application.Permissions.Models;
using Domain.Common;
using Mediator;

namespace Application.Permissions.Queries;

/// <summary>
/// Represents a paginated request to retrieve permissions with optional filtering and sorting.
/// </summary>
public class GetPermissionsQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<PermissionDto>>>;
