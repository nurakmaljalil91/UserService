#nullable enable
using System;

namespace Domain.Entities;

/// <summary>
/// Represents the assignment of a role to a user.
/// </summary>
public class UserRole
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the role identifier.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Gets or sets the user.
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Gets or sets the role.
    /// </summary>
    public Role? Role { get; set; }
}
