#nullable enable
using System;
using System.Globalization;
using Domain.Entities;

namespace Application.UserProfiles.Models;

/// <summary>
/// Represents a user profile summary for API responses.
/// </summary>
public sealed record UserProfileDto
{
    private const string DateFormat = "yyyy-MM-dd";

    /// <summary>
    /// Initializes a new instance of the <see cref="UserProfileDto"/> class.
    /// </summary>
    public UserProfileDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserProfileDto"/> class from a <see cref="UserProfile"/> entity.
    /// </summary>
    /// <param name="profile">The <see cref="UserProfile"/> entity to map from.</param>
    public UserProfileDto(UserProfile profile)
    {
        Id = profile.Id;
        UserId = profile.UserId;
        DisplayName = profile.DisplayName;
        FirstName = profile.FirstName;
        LastName = profile.LastName;
        IdentityCardNumber = profile.IdentityCardNumber;
        PassportNumber = profile.PassportNumber;
        DateOfBirth = profile.DateOfBirth?.ToString(DateFormat, CultureInfo.InvariantCulture);
        BirthPlace = profile.BirthPlace;
        ShoeSize = profile.ShoeSize;
        ClothingSize = profile.ClothingSize;
        WaistSize = profile.WaistSize;
        Bio = profile.Bio;
        ImageUrl = profile.ImageUrl;
        Tag = profile.Tag;
        BloodType = profile.BloodType;
    }

    /// <summary>
    /// Gets the user profile identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Gets the display name shown to other users.
    /// </summary>
    public string? DisplayName { get; init; }

    /// <summary>
    /// Gets the user's first name.
    /// </summary>
    public string? FirstName { get; init; }

    /// <summary>
    /// Gets the user's last name.
    /// </summary>
    public string? LastName { get; init; }

    /// <summary>
    /// Gets the national identity card number.
    /// </summary>
    public string? IdentityCardNumber { get; init; }

    /// <summary>
    /// Gets the passport number.
    /// </summary>
    public string? PassportNumber { get; init; }

    /// <summary>
    /// Gets the date of birth in yyyy-MM-dd format.
    /// </summary>
    public string? DateOfBirth { get; init; }

    /// <summary>
    /// Gets the place of birth.
    /// </summary>
    public string? BirthPlace { get; init; }

    /// <summary>
    /// Gets the shoe size.
    /// </summary>
    public string? ShoeSize { get; init; }

    /// <summary>
    /// Gets the clothing size.
    /// </summary>
    public string? ClothingSize { get; init; }

    /// <summary>
    /// Gets the waist size.
    /// </summary>
    public string? WaistSize { get; init; }

    /// <summary>
    /// Gets the profile bio.
    /// </summary>
    public string? Bio { get; init; }

    /// <summary>
    /// Gets the profile image URL.
    /// </summary>
    public string? ImageUrl { get; init; }

    /// <summary>
    /// Gets the profile tag.
    /// </summary>
    public string? Tag { get; init; }

    /// <summary>
    /// Gets the blood type recorded for the user.
    /// </summary>
    public string? BloodType { get; init; }
}
