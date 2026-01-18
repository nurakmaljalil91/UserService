#nullable enable
using Application.Common.Models;
using Application.UserPreferences.Models;
using Domain.Common;
using Mediator;

namespace Application.UserPreferences.Queries;

/// <summary>
/// Represents a paginated request to retrieve the current user's preferences.
/// </summary>
public class GetMyUserPreferencesQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<UserPreferenceDto>>>
{
}
