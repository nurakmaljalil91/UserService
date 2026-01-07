#nullable enable
using Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Permissions.Commands;

/// <summary>
/// Validates the <see cref="CreatePermissionCommand"/>.
/// </summary>
public class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreatePermissionCommandValidator"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public CreatePermissionCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Permission name is required.")
            .MaximumLength(200).WithMessage("Permission name must not exceed 200 characters.")
            .MustAsync(async (name, cancellationToken) =>
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return true;
                }

                var normalizedName = name.Trim().ToUpperInvariant();
                var exists = await _context.Permissions.AnyAsync(p => p.NormalizedName == normalizedName, cancellationToken);
                return !exists;
            }).WithMessage("Permission name already exists.")
            .WithErrorCode("Unique");

        RuleFor(x => x.Description)
            .MaximumLength(256).WithMessage("Permission description must not exceed 256 characters.");
    }
}
