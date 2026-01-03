#nullable enable
using System;

namespace Domain.Entities;

/// <summary>
/// Represents the assignment of a user to a group.
/// </summary>
public class UserGroup
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the group identifier.
    /// </summary>
    public Guid GroupId { get; set; }

    /// <summary>
    /// Gets or sets the user.
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Gets or sets the group.
    /// </summary>
    public Group? Group { get; set; }
}
