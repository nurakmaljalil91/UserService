#nullable enable
using Application.Addresses.Models;
using Application.Common.Models;
using Domain.Common;
using Mediator;

namespace Application.Addresses.Queries;

/// <summary>
/// Represents a paginated request to retrieve the current user's addresses.
/// </summary>
public class GetMyAddressesQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<AddressDto>>>
{
}
