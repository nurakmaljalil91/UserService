#nullable enable
using System;

namespace Domain.Entities;

/// <summary>
/// Represents the assignment of a role to a group.
/// </summary>
public class GroupRole
{
    /// <summary>
    /// Gets or sets the group identifier.
    /// </summary>
    public Guid GroupId { get; set; }

    /// <summary>
    /// Gets or sets the role identifier.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Gets or sets the group.
    /// </summary>
    public Group? Group { get; set; }

    /// <summary>
    /// Gets or sets the role.
    /// </summary>
    public Role? Role { get; set; }
}
