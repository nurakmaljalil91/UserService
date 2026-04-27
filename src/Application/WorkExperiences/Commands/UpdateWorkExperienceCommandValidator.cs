#nullable enable
using FluentValidation;

namespace Application.WorkExperiences.Commands;

/// <summary>
/// Validates the <see cref="UpdateWorkExperienceCommand"/>.
/// </summary>
public class UpdateWorkExperienceCommandValidator : AbstractValidator<UpdateWorkExperienceCommand>
{
    private const int MaxCompanyLength = 200;
    private const int MaxPositionLength = 150;
    private const int MaxDescriptionLength = 2000;
    private const int MaxLocationLength = 200;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateWorkExperienceCommandValidator"/> class.
    /// </summary>
    public UpdateWorkExperienceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Work experience id is required.");

        RuleFor(x => x.Company)
            .NotEmpty().When(x => x.Company != null).WithMessage("Company must not be empty.")
            .MaximumLength(MaxCompanyLength).When(x => x.Company != null)
            .WithMessage($"Company must not exceed {MaxCompanyLength} characters.");

        RuleFor(x => x.Position)
            .NotEmpty().When(x => x.Position != null).WithMessage("Position must not be empty.")
            .MaximumLength(MaxPositionLength).When(x => x.Position != null)
            .WithMessage($"Position must not exceed {MaxPositionLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(MaxDescriptionLength).When(x => x.Description != null)
            .WithMessage($"Description must not exceed {MaxDescriptionLength} characters.");

        RuleFor(x => x.Location)
            .MaximumLength(MaxLocationLength).When(x => x.Location != null)
            .WithMessage($"Location must not exceed {MaxLocationLength} characters.");
    }
}
