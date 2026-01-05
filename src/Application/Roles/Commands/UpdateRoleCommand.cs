#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Roles.Models;
using Domain.Common;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.Commands;

/// <summary>
/// Command to update an existing role.
/// </summary>
public class UpdateRoleCommand : IRequest<BaseResponse<RoleDto>>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Handles updating a role.
/// </summary>
public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, BaseResponse<RoleDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (role == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Role), request.Id.ToString());
        }

        if (request.Name != null)
        {
            var name = request.Name.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                return BaseResponse<RoleDto>.Fail("Role name is required.");
            }

            var normalizedName = name.ToUpperInvariant();
            var exists = await _context.Roles.AnyAsync(
                r => r.Id != role.Id && r.NormalizedName == normalizedName,
                cancellationToken);

            if (exists)
            {
                return BaseResponse<RoleDto>.Fail("Role name already exists.");
            }

            role.Name = name;
            role.NormalizedName = normalizedName;
        }

        if (request.Description != null)
        {
            role.Description = request.Description.Trim();
        }

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<RoleDto>.Ok(new RoleDto(role), "Role updated.");
    }
}

/// <summary>
/// Validates the <see cref="UpdateRoleCommand"/>.
/// </summary>
public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Role id is required.");

        RuleFor(x => x.Name)
            .MaximumLength(100).When(x => x.Name != null)
            .WithMessage("Role name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(256).When(x => x.Description != null)
            .WithMessage("Role description must not exceed 256 characters.");
    }
}
