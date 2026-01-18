#nullable enable
using Application.UserPreferences.Models;
using Domain.Common;
using Mediator;

namespace Application.UserPreferences.Commands;

/// <summary>
/// Command to update an existing user preference.
/// </summary>
public class UpdateUserPreferenceCommand : IRequest<BaseResponse<UserPreferenceDto>>
{
    /// <summary>
    /// Gets or sets the user preference identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the preference key.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the preference value.
    /// </summary>
    public string? Value { get; set; }
}
