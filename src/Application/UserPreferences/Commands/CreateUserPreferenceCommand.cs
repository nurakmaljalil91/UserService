#nullable enable
using Application.UserPreferences.Models;
using Domain.Common;
using Mediator;

namespace Application.UserPreferences.Commands;

/// <summary>
/// Command to create a new user preference.
/// </summary>
public class CreateUserPreferenceCommand : IRequest<BaseResponse<UserPreferenceDto>>
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the preference key.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the preference value.
    /// </summary>
    public string? Value { get; set; }
}
