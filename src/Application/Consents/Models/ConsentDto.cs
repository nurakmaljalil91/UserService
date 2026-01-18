#nullable enable
using System;
using Domain.Entities;
using NodaTime.Text;

namespace Application.Consents.Models;

/// <summary>
/// Represents a consent summary for API responses.
/// </summary>
public sealed record ConsentDto
{
    private const string InstantPatternText = "yyyy-MM-dd'T'HH:mm:ss'Z'";

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsentDto"/> class.
    /// </summary>
    public ConsentDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsentDto"/> class from a <see cref="Consent"/> entity.
    /// </summary>
    /// <param name="consent">The <see cref="Consent"/> entity to map from.</param>
    public ConsentDto(Consent consent)
    {
        var instantPattern = InstantPattern.CreateWithInvariantCulture(InstantPatternText);
        Id = consent.Id;
        UserId = consent.UserId;
        Type = consent.Type;
        IsGranted = consent.IsGranted;
        GrantedAt = instantPattern.Format(consent.GrantedAt);
        Version = consent.Version;
        Source = consent.Source;
    }

    /// <summary>
    /// Gets the consent identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Gets the consent type.
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Gets whether the consent is granted.
    /// </summary>
    public bool IsGranted { get; init; }

    /// <summary>
    /// Gets when the consent was granted or revoked in UTC.
    /// </summary>
    public string? GrantedAt { get; init; }

    /// <summary>
    /// Gets the consent version.
    /// </summary>
    public string? Version { get; init; }

    /// <summary>
    /// Gets the source of the consent.
    /// </summary>
    public string? Source { get; init; }
}
