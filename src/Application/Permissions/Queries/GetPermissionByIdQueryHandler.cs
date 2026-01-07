#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Permissions.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Permissions.Queries;

/// <summary>
/// Handles retrieval of a permission by identifier.
/// </summary>
public class GetPermissionByIdQueryHandler : IRequestHandler<GetPermissionByIdQuery, BaseResponse<PermissionDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPermissionByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetPermissionByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the permission lookup request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The permission response.</returns>
    public async Task<BaseResponse<PermissionDto>> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
    {
        var permission = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (permission == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Permission), request.Id.ToString());
        }

        return BaseResponse<PermissionDto>.Ok(new PermissionDto(permission), "Permission retrieved.");
    }
}
