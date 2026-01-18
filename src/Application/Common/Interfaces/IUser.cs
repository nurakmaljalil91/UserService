#nullable enable

namespace Application.Common.Interfaces;

/// <summary>
/// Provides information about the current user, including their username and roles.
/// </summary>
public interface IUser
{
    /// <summary>
    /// Gets the unique identifier of the current user, or <c>null</c> if not available.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Gets the username of the current user, or <c>null</c> if not available.
    /// </summary>
    string? Username { get; }

    /// <summary>
    /// Gets the list of roles associated with the current user.
    /// </summary>
    List<string> GetRoles();
}
