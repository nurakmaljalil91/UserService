#nullable enable
using FluentValidation;

namespace Application.Users.Commands;

/// <summary>
/// Validates the <see cref="UnassignRoleFromUserCommand"/>.
/// </summary>
public class UnassignRoleFromUserCommandValidator : AbstractValidator<UnassignRoleFromUserCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnassignRoleFromUserCommandValidator"/> class.
    /// </summary>
    public UnassignRoleFromUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role id is required.");
    }
}
