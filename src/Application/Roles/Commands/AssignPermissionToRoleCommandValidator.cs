#nullable enable
using FluentValidation;

namespace Application.Roles.Commands;

/// <summary>
/// Validates the <see cref="AssignPermissionToRoleCommand"/>.
/// </summary>
public class AssignPermissionToRoleCommandValidator : AbstractValidator<AssignPermissionToRoleCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssignPermissionToRoleCommandValidator"/> class.
    /// </summary>
    public AssignPermissionToRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role id is required.");

        RuleFor(x => x.PermissionId)
            .NotEmpty().WithMessage("Permission id is required.");
    }
}
