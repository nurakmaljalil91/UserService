#nullable enable
using System;

namespace Domain.Entities;

/// <summary>
/// Represents a contact method for a user, such as email or phone.
/// </summary>
public class ContactMethod : BaseEntity<Guid>
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the contact method type.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the contact value (email or phone number).
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the normalized contact value for lookup.
    /// </summary>
    public string? NormalizedValue { get; set; }

    /// <summary>
    /// Gets or sets whether the contact method is verified.
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// Gets or sets whether the contact method is the primary one.
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Gets or sets the user that owns the contact method.
    /// </summary>
    public User? User { get; set; }
}
