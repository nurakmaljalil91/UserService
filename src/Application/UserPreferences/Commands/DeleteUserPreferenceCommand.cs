#nullable enable
using Domain.Common;
using Mediator;

namespace Application.UserPreferences.Commands;

/// <summary>
/// Command to delete a user preference by its identifier.
/// </summary>
public class DeleteUserPreferenceCommand : IRequest<BaseResponse<string>>
{
    /// <summary>
    /// Gets or sets the user preference identifier.
    /// </summary>
    public Guid Id { get; set; }
}
