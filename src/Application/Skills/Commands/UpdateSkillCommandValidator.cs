#nullable enable
using FluentValidation;

namespace Application.Skills.Commands;

/// <summary>
/// Validates the <see cref="UpdateSkillCommand"/>.
/// </summary>
public class UpdateSkillCommandValidator : AbstractValidator<UpdateSkillCommand>
{
    private const int MaxNameLength = 100;
    private const int MaxProficiencyLength = 50;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSkillCommandValidator"/> class.
    /// </summary>
    public UpdateSkillCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Skill id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().When(x => x.Name != null).WithMessage("Skill name must not be empty.")
            .MaximumLength(MaxNameLength).When(x => x.Name != null)
            .WithMessage($"Skill name must not exceed {MaxNameLength} characters.");

        RuleFor(x => x.Proficiency)
            .MaximumLength(MaxProficiencyLength).When(x => x.Proficiency != null)
            .WithMessage($"Proficiency must not exceed {MaxProficiencyLength} characters.");

        RuleFor(x => x.YearsOfExperience)
            .GreaterThanOrEqualTo(0).When(x => x.YearsOfExperience.HasValue)
            .WithMessage("Years of experience must be zero or greater.");
    }
}
