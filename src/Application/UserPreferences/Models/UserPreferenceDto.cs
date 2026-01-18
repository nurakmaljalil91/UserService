#nullable enable
using System;
using Domain.Entities;

namespace Application.UserPreferences.Models;

/// <summary>
/// Represents a user preference summary for API responses.
/// </summary>
public sealed record UserPreferenceDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserPreferenceDto"/> class.
    /// </summary>
    public UserPreferenceDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserPreferenceDto"/> class from a <see cref="UserPreference"/> entity.
    /// </summary>
    /// <param name="preference">The <see cref="UserPreference"/> entity to map from.</param>
    public UserPreferenceDto(UserPreference preference)
    {
        Id = preference.Id;
        UserId = preference.UserId;
        Key = preference.Key;
        Value = preference.Value;
    }

    /// <summary>
    /// Gets the user preference identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Gets the preference key.
    /// </summary>
    public string? Key { get; init; }

    /// <summary>
    /// Gets the preference value.
    /// </summary>
    public string? Value { get; init; }
}
