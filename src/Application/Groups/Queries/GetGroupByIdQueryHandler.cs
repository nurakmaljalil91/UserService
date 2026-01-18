#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Groups.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Groups.Queries;

/// <summary>
/// Handles retrieval of a group by identifier.
/// </summary>
public class GetGroupByIdQueryHandler : IRequestHandler<GetGroupByIdQuery, BaseResponse<GroupDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetGroupByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetGroupByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the group lookup request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The group response.</returns>
    public async Task<BaseResponse<GroupDto>> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await _context.Groups
            .Include(g => g.GroupRoles)
            .ThenInclude(gr => gr.Role)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (group == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Group), request.Id.ToString());
        }

        return BaseResponse<GroupDto>.Ok(new GroupDto(group), "Group retrieved.");
    }
}
