#nullable enable
using FluentValidation;

namespace Application.ContactMethods.Commands;

/// <summary>
/// Validates the <see cref="UpdateContactMethodCommand"/>.
/// </summary>
public class UpdateContactMethodCommandValidator : AbstractValidator<UpdateContactMethodCommand>
{
    private const int MaxTypeLength = 20;
    private const int MaxValueLength = 256;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateContactMethodCommandValidator"/> class.
    /// </summary>
    public UpdateContactMethodCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Contact method id is required.");

        RuleFor(x => x.Type)
            .MaximumLength(MaxTypeLength).When(x => x.Type != null)
            .WithMessage($"Contact method type must not exceed {MaxTypeLength} characters.");

        RuleFor(x => x.Value)
            .MaximumLength(MaxValueLength).When(x => x.Value != null)
            .WithMessage($"Contact value must not exceed {MaxValueLength} characters.");
    }
}
