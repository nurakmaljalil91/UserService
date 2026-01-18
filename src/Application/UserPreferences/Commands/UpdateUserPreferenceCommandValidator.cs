#nullable enable
using FluentValidation;

namespace Application.UserPreferences.Commands;

/// <summary>
/// Validates the <see cref="UpdateUserPreferenceCommand"/>.
/// </summary>
public class UpdateUserPreferenceCommandValidator : AbstractValidator<UpdateUserPreferenceCommand>
{
    private const int MaxKeyLength = 100;
    private const int MaxValueLength = 1024;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserPreferenceCommandValidator"/> class.
    /// </summary>
    public UpdateUserPreferenceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User preference id is required.");

        RuleFor(x => x.Key)
            .MaximumLength(MaxKeyLength).When(x => x.Key != null)
            .WithMessage($"Preference key must not exceed {MaxKeyLength} characters.");

        RuleFor(x => x.Value)
            .MaximumLength(MaxValueLength).When(x => x.Value != null)
            .WithMessage($"Preference value must not exceed {MaxValueLength} characters.");
    }
}
