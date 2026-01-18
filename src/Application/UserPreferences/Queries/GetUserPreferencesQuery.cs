#nullable enable
using Application.Common.Models;
using Application.UserPreferences.Models;
using Domain.Common;
using Mediator;

namespace Application.UserPreferences.Queries;

/// <summary>
/// Represents a paginated request to retrieve user preferences with optional filtering and sorting.
/// </summary>
public class GetUserPreferencesQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<UserPreferenceDto>>>;
