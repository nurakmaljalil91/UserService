#nullable enable
using FluentValidation;

namespace Application.Groups.Commands;

/// <summary>
/// Validates the <see cref="AssignRoleToGroupCommand"/>.
/// </summary>
public class AssignRoleToGroupCommandValidator : AbstractValidator<AssignRoleToGroupCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssignRoleToGroupCommandValidator"/> class.
    /// </summary>
    public AssignRoleToGroupCommandValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("Group id is required.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role id is required.");
    }
}
