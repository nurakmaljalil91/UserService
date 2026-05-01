#nullable enable
using System;
using Domain.Entities;

namespace Application.Languages.Models;

/// <summary>
/// Represents a language record for API responses.
/// </summary>
public sealed record LanguageDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageDto"/> class.
    /// </summary>
    public LanguageDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageDto"/> class from a <see cref="Language"/> entity.
    /// </summary>
    /// <param name="language">The <see cref="Language"/> entity to map from.</param>
    public LanguageDto(Language language)
    {
        Id = language.Id;
        UserId = language.UserId;
        Name = language.Name;
        Level = language.Level;
    }

    /// <summary>
    /// Gets the language identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Gets the language name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the proficiency level.
    /// </summary>
    public string? Level { get; init; }
}
