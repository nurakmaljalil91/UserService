#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Projects.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries;

/// <summary>
/// Handles retrieval of all projects for a specific user.
/// </summary>
public class GetProjectsByUserIdQueryHandler : IRequestHandler<GetProjectsByUserIdQuery, BaseResponse<IEnumerable<ProjectDto>>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProjectsByUserIdQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetProjectsByUserIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the projects lookup request for a user.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of projects for the specified user.</returns>
    public async Task<BaseResponse<IEnumerable<ProjectDto>>> Handle(GetProjectsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var projects = await _context.Projects
            .Where(p => p.UserId == request.UserId)
            .Select(p => new ProjectDto(p))
            .ToListAsync(cancellationToken);

        return BaseResponse<IEnumerable<ProjectDto>>.Ok(projects, $"Successfully retrieved {projects.Count} projects.");
    }
}
