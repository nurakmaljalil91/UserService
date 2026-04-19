#nullable enable
using FluentValidation;

namespace Application.Groups.Commands;

/// <summary>
/// Validates the <see cref="UnassignUserFromGroupCommand"/>.
/// </summary>
public class UnassignUserFromGroupCommandValidator : AbstractValidator<UnassignUserFromGroupCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnassignUserFromGroupCommandValidator"/> class.
    /// </summary>
    public UnassignUserFromGroupCommandValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("Group id is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required.");
    }
}
