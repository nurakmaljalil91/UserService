#nullable enable
using FluentValidation;

namespace Application.UserPreferences.Commands;

/// <summary>
/// Validates the <see cref="CreateUserPreferenceCommand"/>.
/// </summary>
public class CreateUserPreferenceCommandValidator : AbstractValidator<CreateUserPreferenceCommand>
{
    private const int MaxKeyLength = 100;
    private const int MaxValueLength = 1024;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateUserPreferenceCommandValidator"/> class.
    /// </summary>
    public CreateUserPreferenceCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(x => x.Key)
            .NotEmpty().WithMessage("Preference key is required.")
            .MaximumLength(MaxKeyLength).WithMessage($"Preference key must not exceed {MaxKeyLength} characters.");

        RuleFor(x => x.Value)
            .MaximumLength(MaxValueLength).When(x => x.Value != null)
            .WithMessage($"Preference value must not exceed {MaxValueLength} characters.");
    }
}
