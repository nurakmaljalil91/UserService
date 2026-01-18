#nullable enable
using FluentValidation;

namespace Application.Consents.Commands;

/// <summary>
/// Validates the <see cref="CreateConsentCommand"/>.
/// </summary>
public class CreateConsentCommandValidator : AbstractValidator<CreateConsentCommand>
{
    private const int MaxTypeLength = 100;
    private const int MaxVersionLength = 50;
    private const int MaxSourceLength = 50;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateConsentCommandValidator"/> class.
    /// </summary>
    public CreateConsentCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Consent type is required.")
            .MaximumLength(MaxTypeLength).WithMessage($"Consent type must not exceed {MaxTypeLength} characters.");

        RuleFor(x => x.GrantedAt)
            .NotEmpty().WithMessage("GrantedAt is required.");

        RuleFor(x => x.Version)
            .MaximumLength(MaxVersionLength).When(x => x.Version != null)
            .WithMessage($"Consent version must not exceed {MaxVersionLength} characters.");

        RuleFor(x => x.Source)
            .MaximumLength(MaxSourceLength).When(x => x.Source != null)
            .WithMessage($"Consent source must not exceed {MaxSourceLength} characters.");
    }
}
