#nullable enable
using FluentValidation;

namespace Application.Consents.Commands;

/// <summary>
/// Validates the <see cref="UpdateConsentCommand"/>.
/// </summary>
public class UpdateConsentCommandValidator : AbstractValidator<UpdateConsentCommand>
{
    private const int MaxTypeLength = 100;
    private const int MaxVersionLength = 50;
    private const int MaxSourceLength = 50;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateConsentCommandValidator"/> class.
    /// </summary>
    public UpdateConsentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Consent id is required.");

        RuleFor(x => x.Type)
            .MaximumLength(MaxTypeLength).When(x => x.Type != null)
            .WithMessage($"Consent type must not exceed {MaxTypeLength} characters.");

        RuleFor(x => x.Version)
            .MaximumLength(MaxVersionLength).When(x => x.Version != null)
            .WithMessage($"Consent version must not exceed {MaxVersionLength} characters.");

        RuleFor(x => x.Source)
            .MaximumLength(MaxSourceLength).When(x => x.Source != null)
            .WithMessage($"Consent source must not exceed {MaxSourceLength} characters.");
    }
}
