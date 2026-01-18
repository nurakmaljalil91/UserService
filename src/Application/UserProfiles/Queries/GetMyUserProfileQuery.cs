#nullable enable
using Application.UserProfiles.Models;
using Domain.Common;
using Mediator;

namespace Application.UserProfiles.Queries;

/// <summary>
/// Query to retrieve the current user's profile.
/// </summary>
public class GetMyUserProfileQuery : IRequest<BaseResponse<UserProfileDto>>
{
}
