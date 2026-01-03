#nullable enable
using System;

namespace Domain.Entities;

/// <summary>
/// Represents a physical address for a user.
/// </summary>
public class Address : BaseEntity<Guid>
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the address label.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets the address type (home, work, etc.).
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the first address line.
    /// </summary>
    public string? Line1 { get; set; }

    /// <summary>
    /// Gets or sets the second address line.
    /// </summary>
    public string? Line2 { get; set; }

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the state or region.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets whether this address is the default.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Gets or sets the user that owns the address.
    /// </summary>
    public User? User { get; set; }
}
