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
    /// Gets or sets the normalized username for lookup and uniqueness.
    /// </summary>
    public string? NormalizedUsername { get; set; }

    /// <summary>
    /// Gets or sets the email address for the user.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the normalized email for lookup and uniqueness.
    /// </summary>
    public string? NormalizedEmail { get; set; }

    /// <summary>
    /// Gets or sets whether the email is confirmed.
    /// </summary>
    public bool EmailConfirm { get; set; }

    /// <summary>
    /// Gets or sets the phone number for the user.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets whether the phone number is confirmed.
    /// </summary>
    public bool PhoneNumberConfirm { get; set; }

    /// <summary>
    /// Gets or sets whether two-factor authentication is enabled.
    /// </summary>
    public bool TwoFactorEnabled { get; set; }

    /// <summary>
    /// Gets or sets the access failed count.
    /// </summary>
    public int AccessFailedCount { get; set; }

    /// <summary>
    /// Gets or sets whether the user is locked.
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// Gets or sets whether the user is deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the password hash for the user.
    /// </summary>
    public string? PasswordHash { get; set; }

    /// <summary>
    /// Gets the roles assigned directly to the user.
    /// </summary>
    public IList<UserRole> UserRoles { get; private set; } = new List<UserRole>();

    /// <summary>
    /// Gets the groups the user belongs to.
    /// </summary>
    public IList<UserGroup> UserGroups { get; private set; } = new List<UserGroup>();

    /// <summary>
    /// Gets or sets the user's profile details.
    /// </summary>
    public UserProfile? Profile { get; set; }

    /// <summary>
    /// Gets the active sessions for the user.
    /// </summary>
    public IList<Session> Sessions { get; private set; } = new List<Session>();

    /// <summary>
    /// Gets the contact methods associated with the user.
    /// </summary>
    public IList<ContactMethod> ContactMethods { get; private set; } = new List<ContactMethod>();

    /// <summary>
    /// Gets the addresses associated with the user.
    /// </summary>
    public IList<Address> Addresses { get; private set; } = new List<Address>();

    /// <summary>
    /// Gets the login attempts for the user.
    /// </summary>
    public IList<LoginAttempt> LoginAttempts { get; private set; } = new List<LoginAttempt>();

    /// <summary>
    /// Gets the consents recorded for the user.
    /// </summary>
    public IList<Consent> Consents { get; private set; } = new List<Consent>();

    /// <summary>
    /// Gets the preferences associated with the user.
    /// </summary>
    public IList<UserPreference> UserPreferences { get; private set; } = new List<UserPreference>();
}
