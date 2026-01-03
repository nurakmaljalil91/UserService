#nullable enable
using System;
using System.Collections.Generic;

namespace Domain.Entities;

/// <summary>
/// Represents a role that groups permissions for users or groups.
/// </summary>
public class Role : BaseEntity<Guid>
{
    /// <summary>
    /// Gets or sets the role name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the normalized role name for lookup and uniqueness.
    /// </summary>
    public string? NormalizedName { get; set; }

    /// <summary>
    /// Gets or sets the role description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets the user-role assignments.
    /// </summary>
    public IList<UserRole> UserRoles { get; private set; } = new List<UserRole>();

    /// <summary>
    /// Gets the role-permission assignments.
    /// </summary>
    public IList<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    /// <summary>
    /// Gets the group-role assignments.
    /// </summary>
    public IList<GroupRole> GroupRoles { get; private set; } = new List<GroupRole>();
}
