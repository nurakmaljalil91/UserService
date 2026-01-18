#nullable enable
using FluentValidation;

namespace Application.Users.Commands;

/// <summary>
/// Validates the <see cref="AssignRoleToUserCommand"/>.
/// </summary>
public class AssignRoleToUserCommandValidator : AbstractValidator<AssignRoleToUserCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssignRoleToUserCommandValidator"/> class.
    /// </summary>
    public AssignRoleToUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role id is required.");
    }
}
