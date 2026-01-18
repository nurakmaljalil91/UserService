#nullable enable
using Application.UserProfiles.Models;
using Domain.Common;
using Mediator;

namespace Application.UserProfiles.Commands;

/// <summary>
/// Command to create a new user profile.
/// </summary>
public class CreateUserProfileCommand : IRequest<BaseResponse<UserProfileDto>>
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the display name shown to other users.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the national identity card number.
    /// </summary>
    public string? IdentityCardNumber { get; set; }

    /// <summary>
    /// Gets or sets the passport number.
    /// </summary>
    public string? PassportNumber { get; set; }

    /// <summary>
    /// Gets or sets the date of birth in yyyy-MM-dd format.
    /// </summary>
    public string? DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the place of birth.
    /// </summary>
    public string? BirthPlace { get; set; }

    /// <summary>
    /// Gets or sets the shoe size.
    /// </summary>
    public string? ShoeSize { get; set; }

    /// <summary>
    /// Gets or sets the clothing size.
    /// </summary>
    public string? ClothingSize { get; set; }

    /// <summary>
    /// Gets or sets the waist size.
    /// </summary>
    public string? WaistSize { get; set; }

    /// <summary>
    /// Gets or sets the profile bio.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets the profile image URL.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the profile tag.
    /// </summary>
    public string? Tag { get; set; }

    /// <summary>
    /// Gets or sets the blood type recorded for the user.
    /// </summary>
    public string? BloodType { get; set; }
}
