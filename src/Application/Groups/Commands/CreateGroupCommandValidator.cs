#nullable enable
using Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Groups.Commands;

/// <summary>
/// Validates the <see cref="CreateGroupCommand"/>.
/// </summary>
public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    private const int MaxGroupNameLength = 100;
    private const int MaxGroupDescriptionLength = 256;
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateGroupCommandValidator"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public CreateGroupCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Group name is required.")
            .MaximumLength(MaxGroupNameLength).WithMessage($"Group name must not exceed {MaxGroupNameLength} characters.")
            .MustAsync(async (name, cancellationToken) =>
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return true;
                }

                var normalizedName = name.Trim().ToUpperInvariant();
                var exists = await _context.Groups.AnyAsync(g => g.NormalizedName == normalizedName, cancellationToken);
                return !exists;
            }).WithMessage("Group name already exists.")
            .WithErrorCode("Unique");

        RuleFor(x => x.Description)
            .MaximumLength(MaxGroupDescriptionLength)
            .WithMessage($"Group description must not exceed {MaxGroupDescriptionLength} characters.");
    }
}
