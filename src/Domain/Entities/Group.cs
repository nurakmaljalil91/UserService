#nullable enable
using System;
using System.Collections.Generic;

namespace Domain.Entities;

/// <summary>
/// Represents a group of users that can be assigned roles.
/// </summary>
public class Group : BaseEntity<Guid>
{
    /// <summary>
    /// Gets or sets the group name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the normalized group name for lookup and uniqueness.
    /// </summary>
    public string? NormalizedName { get; set; }

    /// <summary>
    /// Gets or sets the group description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets the user-group assignments.
    /// </summary>
    public IList<UserGroup> UserGroups { get; private set; } = new List<UserGroup>();

    /// <summary>
    /// Gets the group-role assignments.
    /// </summary>
    public IList<GroupRole> GroupRoles { get; private set; } = new List<GroupRole>();
}
