#nullable enable
using FluentValidation;

namespace Application.Projects.Commands;

/// <summary>
/// Validates the <see cref="UpdateProjectCommand"/>.
/// </summary>
public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    private const int MaxTitleLength = 200;
    private const int MaxDescriptionLength = 2000;
    private const int MaxUrlLength = 2048;
    private const int MaxTechStackLength = 500;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProjectCommandValidator"/> class.
    /// </summary>
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Project id is required.");

        RuleFor(x => x.Title)
            .NotEmpty().When(x => x.Title != null).WithMessage("Title must not be empty.")
            .MaximumLength(MaxTitleLength).When(x => x.Title != null)
            .WithMessage($"Title must not exceed {MaxTitleLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(MaxDescriptionLength).When(x => x.Description != null)
            .WithMessage($"Description must not exceed {MaxDescriptionLength} characters.");

        RuleFor(x => x.Url)
            .MaximumLength(MaxUrlLength).When(x => x.Url != null)
            .WithMessage($"URL must not exceed {MaxUrlLength} characters.");

        RuleFor(x => x.TechStack)
            .MaximumLength(MaxTechStackLength).When(x => x.TechStack != null)
            .WithMessage($"Tech stack must not exceed {MaxTechStackLength} characters.");
    }
}
