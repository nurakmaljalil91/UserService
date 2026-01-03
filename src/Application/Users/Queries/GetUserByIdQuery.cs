#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Users.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries;

/// <summary>
/// Query to retrieve a user by identifier.
/// </summary>
public class GetUserByIdQuery : IRequest<BaseResponse<UserDto>>
{
    public Guid Id { get; set; }
}

/// <summary>
/// Handles retrieval of a user by identifier.
/// </summary>
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, BaseResponse<UserDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUserByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserGroups)
            .ThenInclude(ug => ug.Group)
            .ThenInclude(g => g.GroupRoles)
            .ThenInclude(gr => gr.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.User), request.Id.ToString());
        }

        return BaseResponse<UserDto>.Ok(new UserDto(user), "User retrieved.");
    }
}
