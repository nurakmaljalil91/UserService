#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Projects.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Commands;

/// <summary>
/// Handles updating an existing project record.
/// </summary>
public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, BaseResponse<ProjectDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IClockService _clockService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProjectCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="clockService">The clock service for parsing dates.</param>
    public UpdateProjectCommandHandler(IApplicationDbContext context, IClockService clockService)
    {
        _context = context;
        _clockService = clockService;
    }

    /// <summary>
    /// Handles the project record update request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated project record response.</returns>
    public async Task<BaseResponse<ProjectDto>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (project == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Project), request.Id.ToString());
        }

        if (request.Title != null)
        {
            project.Title = request.Title.Trim();
        }

        if (request.Description != null)
        {
            project.Description = request.Description.Trim();
        }

        if (request.StartDate != null)
        {
            if (string.IsNullOrWhiteSpace(request.StartDate))
            {
                project.StartDate = null;
            }
            else
            {
                var parsed = _clockService.TryParseDate(request.StartDate);
                if (parsed == null || !parsed.Success)
                {
                    return BaseResponse<ProjectDto>.Fail("Start date must be in yyyy-MM-dd format.");
                }

                project.StartDate = parsed.Value;
            }
        }

        if (request.EndDate != null)
        {
            if (string.IsNullOrWhiteSpace(request.EndDate))
            {
                project.EndDate = null;
            }
            else
            {
                var parsed = _clockService.TryParseDate(request.EndDate);
                if (parsed == null || !parsed.Success)
                {
                    return BaseResponse<ProjectDto>.Fail("End date must be in yyyy-MM-dd format.");
                }

                project.EndDate = parsed.Value;
            }
        }

        if (request.Url != null)
        {
            project.Url = request.Url.Trim();
        }

        if (request.TechStack != null)
        {
            project.TechStack = request.TechStack.Trim();
        }

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<ProjectDto>.Ok(new ProjectDto(project), "Project updated.");
    }
}
