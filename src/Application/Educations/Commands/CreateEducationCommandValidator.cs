#nullable enable
using FluentValidation;

namespace Application.Educations.Commands;

/// <summary>
/// Validates the <see cref="CreateEducationCommand"/>.
/// </summary>
public class CreateEducationCommandValidator : AbstractValidator<CreateEducationCommand>
{
    private const int MaxInstitutionLength = 200;
    private const int MaxDegreeLength = 150;
    private const int MaxFieldOfStudyLength = 150;
    private const int MaxGradeLength = 50;
    private const int MaxDescriptionLength = 1000;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateEducationCommandValidator"/> class.
    /// </summary>
    public CreateEducationCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(x => x.Institution)
            .NotEmpty().WithMessage("Institution is required.")
            .MaximumLength(MaxInstitutionLength).WithMessage($"Institution must not exceed {MaxInstitutionLength} characters.");

        RuleFor(x => x.Degree)
            .MaximumLength(MaxDegreeLength).When(x => x.Degree != null)
            .WithMessage($"Degree must not exceed {MaxDegreeLength} characters.");

        RuleFor(x => x.FieldOfStudy)
            .MaximumLength(MaxFieldOfStudyLength).When(x => x.FieldOfStudy != null)
            .WithMessage($"Field of study must not exceed {MaxFieldOfStudyLength} characters.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(x => x.Grade)
            .MaximumLength(MaxGradeLength).When(x => x.Grade != null)
            .WithMessage($"Grade must not exceed {MaxGradeLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(MaxDescriptionLength).When(x => x.Description != null)
            .WithMessage($"Description must not exceed {MaxDescriptionLength} characters.");
    }
}
