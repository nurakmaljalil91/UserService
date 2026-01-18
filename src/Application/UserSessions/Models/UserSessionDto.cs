#nullable enable
using System.Collections.Generic;
using Application.UserPreferences.Models;
using Application.UserProfiles.Models;
using Application.Users.Models;

namespace Application.UserSessions.Models;

/// <summary>
/// Represents a user session response containing user context and preferences.
/// </summary>
public sealed record UserSessionDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserSessionDto"/> class.
    /// </summary>
    /// <param name="user">The user details.</param>
    /// <param name="profile">The user profile details.</param>
    /// <param name="preferences">The user preference entries.</param>
    public UserSessionDto(UserDto user, UserProfileDto profile, IReadOnlyCollection<UserPreferenceDto> preferences)
    {
        User = user;
        Profile = profile;
        Preferences = preferences;
        Roles = user.Roles;
        Permissions = user.Permissions;
        Groups = user.Groups;
        GroupRoles = user.GroupRoles;
    }

    /// <summary>
    /// Gets the user details.
    /// </summary>
    public UserDto User { get; }

    /// <summary>
    /// Gets the user profile details.
    /// </summary>
    public UserProfileDto Profile { get; }

    /// <summary>
    /// Gets the user preference entries.
    /// </summary>
    public IReadOnlyCollection<UserPreferenceDto> Preferences { get; }

    /// <summary>
    /// Gets the role names assigned to the user.
    /// </summary>
    public IReadOnlyCollection<string> Roles { get; }

    /// <summary>
    /// Gets the permission names available to the user.
    /// </summary>
    public IReadOnlyCollection<string> Permissions { get; }

    /// <summary>
    /// Gets the group names the user belongs to.
    /// </summary>
    public IReadOnlyCollection<string> Groups { get; }

    /// <summary>
    /// Gets the roles grouped by group name.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> GroupRoles { get; }
}
