#nullable enable
using Application.Common.Models;
using Application.Consents.Models;
using Domain.Common;
using Mediator;

namespace Application.Consents.Queries;

/// <summary>
/// Represents a paginated request to retrieve the current user's consents.
/// </summary>
public class GetMyConsentsQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<ConsentDto>>>
{
}
