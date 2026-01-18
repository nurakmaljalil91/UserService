#nullable enable
using Application.Addresses.Models;
using Application.Common.Models;
using Domain.Common;
using Mediator;

namespace Application.Addresses.Queries;

/// <summary>
/// Represents a paginated request to retrieve addresses with optional filtering and sorting.
/// </summary>
public class GetAddressesQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<AddressDto>>>;
