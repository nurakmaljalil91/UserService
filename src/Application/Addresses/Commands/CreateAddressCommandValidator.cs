#nullable enable
using FluentValidation;

namespace Application.Addresses.Commands;

/// <summary>
/// Validates the <see cref="CreateAddressCommand"/>.
/// </summary>
public class CreateAddressCommandValidator : AbstractValidator<CreateAddressCommand>
{
    private const int MaxLabelLength = 100;
    private const int MaxTypeLength = 30;
    private const int MaxLineLength = 200;
    private const int MaxCityLength = 100;
    private const int MaxStateLength = 100;
    private const int MaxPostalCodeLength = 20;
    private const int MaxCountryLength = 100;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateAddressCommandValidator"/> class.
    /// </summary>
    public CreateAddressCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(x => x.Line1)
            .NotEmpty().WithMessage("Address line1 is required.")
            .MaximumLength(MaxLineLength).WithMessage($"Address line1 must not exceed {MaxLineLength} characters.");

        RuleFor(x => x.Label)
            .MaximumLength(MaxLabelLength).When(x => x.Label != null)
            .WithMessage($"Address label must not exceed {MaxLabelLength} characters.");

        RuleFor(x => x.Type)
            .MaximumLength(MaxTypeLength).When(x => x.Type != null)
            .WithMessage($"Address type must not exceed {MaxTypeLength} characters.");

        RuleFor(x => x.Line2)
            .MaximumLength(MaxLineLength).When(x => x.Line2 != null)
            .WithMessage($"Address line2 must not exceed {MaxLineLength} characters.");

        RuleFor(x => x.City)
            .MaximumLength(MaxCityLength).When(x => x.City != null)
            .WithMessage($"City must not exceed {MaxCityLength} characters.");

        RuleFor(x => x.State)
            .MaximumLength(MaxStateLength).When(x => x.State != null)
            .WithMessage($"State must not exceed {MaxStateLength} characters.");

        RuleFor(x => x.PostalCode)
            .MaximumLength(MaxPostalCodeLength).When(x => x.PostalCode != null)
            .WithMessage($"Postal code must not exceed {MaxPostalCodeLength} characters.");

        RuleFor(x => x.Country)
            .MaximumLength(MaxCountryLength).When(x => x.Country != null)
            .WithMessage($"Country must not exceed {MaxCountryLength} characters.");
    }
}
