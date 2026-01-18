#nullable enable
using System;
using Domain.Entities;

namespace Application.Addresses.Models;

/// <summary>
/// Represents an address summary for API responses.
/// </summary>
public sealed record AddressDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddressDto"/> class.
    /// </summary>
    public AddressDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AddressDto"/> class from an <see cref="Address"/> entity.
    /// </summary>
    /// <param name="address">The <see cref="Address"/> entity to map from.</param>
    public AddressDto(Address address)
    {
        Id = address.Id;
        UserId = address.UserId;
        Label = address.Label;
        Type = address.Type;
        Line1 = address.Line1;
        Line2 = address.Line2;
        City = address.City;
        State = address.State;
        PostalCode = address.PostalCode;
        Country = address.Country;
        IsDefault = address.IsDefault;
    }

    /// <summary>
    /// Gets the address identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Gets the address label.
    /// </summary>
    public string? Label { get; init; }

    /// <summary>
    /// Gets the address type (home, work, etc.).
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Gets the first address line.
    /// </summary>
    public string? Line1 { get; init; }

    /// <summary>
    /// Gets the second address line.
    /// </summary>
    public string? Line2 { get; init; }

    /// <summary>
    /// Gets the city.
    /// </summary>
    public string? City { get; init; }

    /// <summary>
    /// Gets the state or region.
    /// </summary>
    public string? State { get; init; }

    /// <summary>
    /// Gets the postal code.
    /// </summary>
    public string? PostalCode { get; init; }

    /// <summary>
    /// Gets the country.
    /// </summary>
    public string? Country { get; init; }

    /// <summary>
    /// Gets whether this address is the default.
    /// </summary>
    public bool IsDefault { get; init; }
}
