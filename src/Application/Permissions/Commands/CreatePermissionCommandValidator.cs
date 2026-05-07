#nullable enable
using FluentValidation;

namespace Application.Permissions.Commands;

/// <summary>
/// Validates the <see cref="CreatePermissionCommand"/>.
/// </summary>
public class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreatePermissionCommandValidator"/> class.
    /// </summary>
    public CreatePermissionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Permission name is required.")
            .MaximumLength(200).WithMessage("Permission name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(256).WithMessage("Permission description must not exceed 256 characters.");
    }
}
