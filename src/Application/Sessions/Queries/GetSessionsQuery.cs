#nullable enable
using Application.Common.Models;
using Application.Sessions.Models;
using Domain.Common;
using Mediator;

namespace Application.Sessions.Queries;

/// <summary>
/// Represents a paginated request to retrieve sessions with optional filtering and sorting.
/// </summary>
public class GetSessionsQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<SessionDto>>>;
