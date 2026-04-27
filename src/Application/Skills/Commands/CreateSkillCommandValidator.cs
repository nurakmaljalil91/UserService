#nullable enable
using FluentValidation;

namespace Application.Skills.Commands;

/// <summary>
/// Validates the <see cref="CreateSkillCommand"/>.
/// </summary>
public class CreateSkillCommandValidator : AbstractValidator<CreateSkillCommand>
{
    private const int MaxNameLength = 100;
    private const int MaxProficiencyLength = 50;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSkillCommandValidator"/> class.
    /// </summary>
    public CreateSkillCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Skill name is required.")
            .MaximumLength(MaxNameLength).WithMessage($"Skill name must not exceed {MaxNameLength} characters.");

        RuleFor(x => x.Proficiency)
            .MaximumLength(MaxProficiencyLength).When(x => x.Proficiency != null)
            .WithMessage($"Proficiency must not exceed {MaxProficiencyLength} characters.");

        RuleFor(x => x.YearsOfExperience)
            .GreaterThanOrEqualTo(0).When(x => x.YearsOfExperience.HasValue)
            .WithMessage("Years of experience must be zero or greater.");
    }
}
