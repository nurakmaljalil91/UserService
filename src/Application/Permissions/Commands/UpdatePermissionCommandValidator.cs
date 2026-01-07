#nullable enable
using FluentValidation;

namespace Application.Permissions.Commands;

/// <summary>
/// Validates the <see cref="UpdatePermissionCommand"/>.
/// </summary>
public class UpdatePermissionCommandValidator : AbstractValidator<UpdatePermissionCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatePermissionCommandValidator"/> class.
    /// </summary>
    public UpdatePermissionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Permission id is required.");

        RuleFor(x => x.Name)
            .MaximumLength(200).When(x => x.Name != null)
            .WithMessage("Permission name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(256).When(x => x.Description != null)
            .WithMessage("Permission description must not exceed 256 characters.");
    }
}
