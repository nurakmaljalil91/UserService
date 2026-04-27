#nullable enable
using FluentValidation;

namespace Application.WorkExperiences.Commands;

/// <summary>
/// Validates the <see cref="CreateWorkExperienceCommand"/>.
/// </summary>
public class CreateWorkExperienceCommandValidator : AbstractValidator<CreateWorkExperienceCommand>
{
    private const int MaxCompanyLength = 200;
    private const int MaxPositionLength = 150;
    private const int MaxDescriptionLength = 2000;
    private const int MaxLocationLength = 200;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateWorkExperienceCommandValidator"/> class.
    /// </summary>
    public CreateWorkExperienceCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(x => x.Company)
            .NotEmpty().WithMessage("Company is required.")
            .MaximumLength(MaxCompanyLength).WithMessage($"Company must not exceed {MaxCompanyLength} characters.");

        RuleFor(x => x.Position)
            .NotEmpty().WithMessage("Position is required.")
            .MaximumLength(MaxPositionLength).WithMessage($"Position must not exceed {MaxPositionLength} characters.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(x => x.Description)
            .MaximumLength(MaxDescriptionLength).When(x => x.Description != null)
            .WithMessage($"Description must not exceed {MaxDescriptionLength} characters.");

        RuleFor(x => x.Location)
            .MaximumLength(MaxLocationLength).When(x => x.Location != null)
            .WithMessage($"Location must not exceed {MaxLocationLength} characters.");
    }
}
