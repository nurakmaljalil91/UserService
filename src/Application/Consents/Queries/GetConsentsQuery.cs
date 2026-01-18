#nullable enable
using Application.Common.Models;
using Application.Consents.Models;
using Domain.Common;
using Mediator;

namespace Application.Consents.Queries;

/// <summary>
/// Represents a paginated request to retrieve consents with optional filtering and sorting.
/// </summary>
public class GetConsentsQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<ConsentDto>>>;
