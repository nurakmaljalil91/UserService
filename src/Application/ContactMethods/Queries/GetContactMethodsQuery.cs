#nullable enable
using Application.Common.Models;
using Application.ContactMethods.Models;
using Domain.Common;
using Mediator;

namespace Application.ContactMethods.Queries;

/// <summary>
/// Represents a paginated request to retrieve contact methods with optional filtering and sorting.
/// </summary>
public class GetContactMethodsQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<ContactMethodDto>>>;
