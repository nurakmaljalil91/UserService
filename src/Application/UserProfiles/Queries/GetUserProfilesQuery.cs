#nullable enable
using Application.Common.Models;
using Application.UserProfiles.Models;
using Domain.Common;
using Mediator;

namespace Application.UserProfiles.Queries;

/// <summary>
/// Represents a paginated request to retrieve user profiles with optional filtering and sorting.
/// </summary>
public class GetUserProfilesQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<UserProfileDto>>>;
