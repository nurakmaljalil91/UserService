#nullable enable
using Domain.Common;
using Mediator;

namespace Application.UserProfiles.Commands;

/// <summary>
/// Command to delete a user profile by its identifier.
/// </summary>
public class DeleteUserProfileCommand : IRequest<BaseResponse<string>>
{
    /// <summary>
    /// Gets or sets the user profile identifier.
    /// </summary>
    public Guid Id { get; set; }
}
