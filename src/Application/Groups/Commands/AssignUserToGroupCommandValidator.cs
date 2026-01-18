#nullable enable
using FluentValidation;

namespace Application.Groups.Commands;

/// <summary>
/// Validates the <see cref="AssignUserToGroupCommand"/>.
/// </summary>
public class AssignUserToGroupCommandValidator : AbstractValidator<AssignUserToGroupCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssignUserToGroupCommandValidator"/> class.
    /// </summary>
    public AssignUserToGroupCommandValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("Group id is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required.");
    }
}
