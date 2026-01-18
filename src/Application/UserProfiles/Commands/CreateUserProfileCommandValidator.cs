#nullable enable
using FluentValidation;

namespace Application.UserProfiles.Commands;

/// <summary>
/// Validates the <see cref="CreateUserProfileCommand"/>.
/// </summary>
public class CreateUserProfileCommandValidator : AbstractValidator<CreateUserProfileCommand>
{
    private const int MaxDisplayNameLength = 150;
    private const int MaxFirstNameLength = 100;
    private const int MaxLastNameLength = 100;
    private const int MaxIdentityCardNumberLength = 50;
    private const int MaxPassportNumberLength = 50;
    private const int MaxBirthPlaceLength = 150;
    private const int MaxShoeSizeLength = 20;
    private const int MaxClothingSizeLength = 20;
    private const int MaxWaistSizeLength = 20;
    private const int MaxBioLength = 512;
    private const int MaxImageUrlLength = 2048;
    private const int MaxTagLength = 100;
    private const int MaxBloodTypeLength = 10;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateUserProfileCommandValidator"/> class.
    /// </summary>
    public CreateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(x => x.DisplayName)
            .MaximumLength(MaxDisplayNameLength).When(x => x.DisplayName != null)
            .WithMessage($"Display name must not exceed {MaxDisplayNameLength} characters.");

        RuleFor(x => x.FirstName)
            .MaximumLength(MaxFirstNameLength).When(x => x.FirstName != null)
            .WithMessage($"First name must not exceed {MaxFirstNameLength} characters.");

        RuleFor(x => x.LastName)
            .MaximumLength(MaxLastNameLength).When(x => x.LastName != null)
            .WithMessage($"Last name must not exceed {MaxLastNameLength} characters.");

        RuleFor(x => x.IdentityCardNumber)
            .MaximumLength(MaxIdentityCardNumberLength).When(x => x.IdentityCardNumber != null)
            .WithMessage($"Identity card number must not exceed {MaxIdentityCardNumberLength} characters.");

        RuleFor(x => x.PassportNumber)
            .MaximumLength(MaxPassportNumberLength).When(x => x.PassportNumber != null)
            .WithMessage($"Passport number must not exceed {MaxPassportNumberLength} characters.");

        RuleFor(x => x.BirthPlace)
            .MaximumLength(MaxBirthPlaceLength).When(x => x.BirthPlace != null)
            .WithMessage($"Birth place must not exceed {MaxBirthPlaceLength} characters.");

        RuleFor(x => x.ShoeSize)
            .MaximumLength(MaxShoeSizeLength).When(x => x.ShoeSize != null)
            .WithMessage($"Shoe size must not exceed {MaxShoeSizeLength} characters.");

        RuleFor(x => x.ClothingSize)
            .MaximumLength(MaxClothingSizeLength).When(x => x.ClothingSize != null)
            .WithMessage($"Clothing size must not exceed {MaxClothingSizeLength} characters.");

        RuleFor(x => x.WaistSize)
            .MaximumLength(MaxWaistSizeLength).When(x => x.WaistSize != null)
            .WithMessage($"Waist size must not exceed {MaxWaistSizeLength} characters.");

        RuleFor(x => x.Bio)
            .MaximumLength(MaxBioLength).When(x => x.Bio != null)
            .WithMessage($"Bio must not exceed {MaxBioLength} characters.");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(MaxImageUrlLength).When(x => x.ImageUrl != null)
            .WithMessage($"Image URL must not exceed {MaxImageUrlLength} characters.");

        RuleFor(x => x.Tag)
            .MaximumLength(MaxTagLength).When(x => x.Tag != null)
            .WithMessage($"Tag must not exceed {MaxTagLength} characters.");

        RuleFor(x => x.BloodType)
            .MaximumLength(MaxBloodTypeLength).When(x => x.BloodType != null)
            .WithMessage($"Blood type must not exceed {MaxBloodTypeLength} characters.");
    }
}
