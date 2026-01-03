#nullable enable
using System;

namespace Domain.Entities;

/// <summary>
/// Represents a user preference stored as a key/value pair.
/// </summary>
public class UserPreference : BaseEntity<Guid>
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

    /// <summary>
    /// Gets or sets the user that owns the preference.
    /// </summary>
    public User? User { get; set; }
}
