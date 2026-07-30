#nullable enable
using Application.Common.Interfaces;
using Application.Permissions.Models;
using Domain.Common;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Permissions.Commands;

/// <summary>
/// Handles creation of a new permission.
/// </summary>
public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, BaseResponse<PermissionDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreatePermissionCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public CreatePermissionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the creation of a new permission.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created permission response.</returns>
    public async Task<BaseResponse<PermissionDto>> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name!.Trim();
        var normalizedName = name.ToUpperInvariant();

        var exists = await _context.Permissions.AnyAsync(
            p => p.NormalizedName == normalizedName,
            cancellationToken);

        if (exists)
        {
            return BaseResponse<PermissionDto>.Fail("Permission name already exists.");
        }

        var permission = new Permission
        {
            Name = name,
            NormalizedName = normalizedName,
            Description = request.Description?.Trim()
        };

        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<PermissionDto>.Ok(new PermissionDto(permission), $"Created permission with id {permission.Id}");
    }
}
