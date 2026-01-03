#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities;

/// <summary>
/// Represents an application user with authentication credentials.
/// </summary>
public class User : BaseEntity<Guid>
{
    /// <summary>
    /// Gets or sets the unique username for the user.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the email address for the user.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the password hash for the user.
    /// </summary>
    public string? PasswordHash { get; set; }
}
