#nullable enable
using Application.Common.Models;
using Application.Sessions.Models;
using Domain.Common;
using Mediator;

namespace Application.Sessions.Queries;

/// <summary>
/// Represents a paginated request to retrieve the current user's sessions.
/// </summary>
public class GetMySessionsQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<SessionDto>>>
{
}
