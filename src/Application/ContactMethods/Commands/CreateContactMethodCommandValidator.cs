#nullable enable
using FluentValidation;

namespace Application.ContactMethods.Commands;

/// <summary>
/// Validates the <see cref="CreateContactMethodCommand"/>.
/// </summary>
public class CreateContactMethodCommandValidator : AbstractValidator<CreateContactMethodCommand>
{
    private const int MaxTypeLength = 20;
    private const int MaxValueLength = 256;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateContactMethodCommandValidator"/> class.
    /// </summary>
    public CreateContactMethodCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Contact method type is required.")
            .MaximumLength(MaxTypeLength).WithMessage($"Contact method type must not exceed {MaxTypeLength} characters.");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Contact value is required.")
            .MaximumLength(MaxValueLength).WithMessage($"Contact value must not exceed {MaxValueLength} characters.");
    }
}
