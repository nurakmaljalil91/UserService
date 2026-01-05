#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Roles.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.Queries;

/// <summary>
/// Query to retrieve a role by identifier.
/// </summary>
public class GetRoleByIdQuery : IRequest<BaseResponse<RoleDto>>
{
    public Guid Id { get; set; }
}

/// <summary>
/// Handles retrieval of a role by identifier.
/// </summary>
public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, BaseResponse<RoleDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRoleByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (role == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Role), request.Id.ToString());
        }

        return BaseResponse<RoleDto>.Ok(new RoleDto(role), "Role retrieved.");
    }
}
