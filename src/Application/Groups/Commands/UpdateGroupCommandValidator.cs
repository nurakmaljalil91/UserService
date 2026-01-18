#nullable enable
using FluentValidation;

namespace Application.Groups.Commands;

/// <summary>
/// Validates the <see cref="UpdateGroupCommand"/>.
/// </summary>
public class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand>
{
    private const int MaxGroupNameLength = 100;
    private const int MaxGroupDescriptionLength = 256;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateGroupCommandValidator"/> class.
    /// </summary>
    public UpdateGroupCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Group id is required.");

        RuleFor(x => x.Name)
            .MaximumLength(MaxGroupNameLength).When(x => x.Name != null)
            .WithMessage($"Group name must not exceed {MaxGroupNameLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(MaxGroupDescriptionLength).When(x => x.Description != null)
            .WithMessage($"Group description must not exceed {MaxGroupDescriptionLength} characters.");
    }
}
