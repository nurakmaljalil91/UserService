#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Permissions.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Permissions.Commands;

/// <summary>
/// Handles updating a permission.
/// </summary>
public class UpdatePermissionCommandHandler : IRequestHandler<UpdatePermissionCommand, BaseResponse<PermissionDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatePermissionCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UpdatePermissionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the permission update request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated permission response.</returns>
    public async Task<BaseResponse<PermissionDto>> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (permission == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Permission), request.Id.ToString());
        }

        if (request.Name != null)
        {
            var name = request.Name.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                return BaseResponse<PermissionDto>.Fail("Permission name is required.");
            }

            var normalizedName = name.ToUpperInvariant();
            var exists = await _context.Permissions.AnyAsync(
                p => p.Id != permission.Id && p.NormalizedName == normalizedName,
                cancellationToken);

            if (exists)
            {
                return BaseResponse<PermissionDto>.Fail("Permission name already exists.");
            }

            permission.Name = name;
            permission.NormalizedName = normalizedName;
        }

        if (request.Description != null)
        {
            permission.Description = request.Description.Trim();
        }

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<PermissionDto>.Ok(new PermissionDto(permission), "Permission updated.");
    }
}
