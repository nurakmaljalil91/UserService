#nullable enable
using FluentValidation;

namespace Application.Groups.Commands;

/// <summary>
/// Validates the <see cref="UnassignRoleFromGroupCommand"/>.
/// </summary>
public class UnassignRoleFromGroupCommandValidator : AbstractValidator<UnassignRoleFromGroupCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnassignRoleFromGroupCommandValidator"/> class.
    /// </summary>
    public UnassignRoleFromGroupCommandValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("Group id is required.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role id is required.");
    }
}
