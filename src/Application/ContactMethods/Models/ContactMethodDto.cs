#nullable enable
using System;
using Domain.Entities;

namespace Application.ContactMethods.Models;

/// <summary>
/// Represents a contact method summary for API responses.
/// </summary>
public sealed record ContactMethodDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContactMethodDto"/> class.
    /// </summary>
    public ContactMethodDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContactMethodDto"/> class from a <see cref="ContactMethod"/> entity.
    /// </summary>
    /// <param name="contactMethod">The <see cref="ContactMethod"/> entity to map from.</param>
    public ContactMethodDto(ContactMethod contactMethod)
    {
        Id = contactMethod.Id;
        UserId = contactMethod.UserId;
        Type = contactMethod.Type;
        Value = contactMethod.Value;
        NormalizedValue = contactMethod.NormalizedValue;
        IsVerified = contactMethod.IsVerified;
        IsPrimary = contactMethod.IsPrimary;
    }

    /// <summary>
    /// Gets the contact method identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Gets the contact method type.
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Gets the contact value (email or phone number).
    /// </summary>
    public string? Value { get; init; }

    /// <summary>
    /// Gets the normalized contact value for lookup.
    /// </summary>
    public string? NormalizedValue { get; init; }

    /// <summary>
    /// Gets whether the contact method is verified.
    /// </summary>
    public bool IsVerified { get; init; }

    /// <summary>
    /// Gets whether the contact method is the primary one.
    /// </summary>
    public bool IsPrimary { get; init; }
}
