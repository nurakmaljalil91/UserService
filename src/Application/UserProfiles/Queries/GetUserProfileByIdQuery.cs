#nullable enable
using Application.UserProfiles.Models;
using Domain.Common;
using Mediator;

namespace Application.UserProfiles.Queries;

/// <summary>
/// Query to retrieve a user profile by identifier.
/// </summary>
public class GetUserProfileByIdQuery : IRequest<BaseResponse<UserProfileDto>>
{
    /// <summary>
    /// Gets or sets the user profile identifier.
    /// </summary>
    public Guid Id { get; set; }
}
