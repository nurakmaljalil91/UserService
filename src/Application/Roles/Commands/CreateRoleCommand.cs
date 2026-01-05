#nullable enable
using Application.Common.Interfaces;
using Application.Roles.Models;
using Domain.Common;
using Domain.Entities;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.Commands;

/// <summary>
/// Command to create a new role.
/// </summary>
public class CreateRoleCommand : IRequest<BaseResponse<RoleDto>>
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Handles creation of a new role.
/// </summary>
public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, BaseResponse<RoleDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name!.Trim();
        var normalizedName = name.ToUpperInvariant();

        var role = new Role
        {
            Name = name,
            NormalizedName = normalizedName,
            Description = request.Description?.Trim()
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<RoleDto>.Ok(new RoleDto(role), $"Created role with id {role.Id}");
    }
}

/// <summary>
/// Validates the <see cref="CreateRoleCommand"/>.
/// </summary>
public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateRoleCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name is required.")
            .MaximumLength(100).WithMessage("Role name must not exceed 100 characters.")
            .MustAsync(async (name, cancellationToken) =>
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return true;
                }

                var normalizedName = name.Trim().ToUpperInvariant();
                var exists = await _context.Roles.AnyAsync(r => r.NormalizedName == normalizedName, cancellationToken);
                return !exists;
            }).WithMessage("Role name already exists.")
            .WithErrorCode("Unique");

        RuleFor(x => x.Description)
            .MaximumLength(256).WithMessage("Role description must not exceed 256 characters.");
    }
}
