#nullable enable
using FluentValidation;

namespace Application.Projects.Commands;

/// <summary>
/// Validates the <see cref="CreateProjectCommand"/>.
/// </summary>
public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    private const int MaxTitleLength = 200;
    private const int MaxDescriptionLength = 2000;
    private const int MaxUrlLength = 2048;
    private const int MaxTechStackLength = 500;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProjectCommandValidator"/> class.
    /// </summary>
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(MaxTitleLength).WithMessage($"Title must not exceed {MaxTitleLength} characters.");

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
