#nullable enable
using System;
using System.Collections.Generic;

namespace Domain.Entities;

/// <summary>
/// Represents a permission granted by roles.
/// </summary>
public class Permission : BaseEntity<Guid>
{
    /// <summary>
    /// Gets or sets the permission name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the normalized permission name for lookup and uniqueness.
    /// </summary>
    public string? NormalizedName { get; set; }

    /// <summary>
    /// Gets or sets the permission description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets the role-permission assignments.
    /// </summary>
    public IList<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();
}
