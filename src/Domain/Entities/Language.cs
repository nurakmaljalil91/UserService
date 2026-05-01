#nullable enable
using System;

namespace Domain.Entities;

/// <summary>
/// Represents a language spoken by a user.
/// </summary>
public class Language : BaseEntity<Guid>
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the language name (e.g., English, Malay, French).
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the proficiency level (e.g., Native, Fluent, Work Proficiency, Conversational, Basic).
    /// </summary>
    public string? Level { get; set; }

    /// <summary>
    /// Gets or sets the user that owns this language record.
    /// </summary>
    public User? User { get; set; }
}
