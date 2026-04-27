#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Projects.Models;
using Domain.Common;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Commands;

/// <summary>
/// Handles creation of a new project record.
/// </summary>
public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, BaseResponse<ProjectDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IClockService _clockService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProjectCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="clockService">The clock service for parsing dates.</param>
    public CreateProjectCommandHandler(IApplicationDbContext context, IClockService clockService)
    {
        _context = context;
        _clockService = clockService;
    }

    /// <summary>
    /// Handles the creation of a new project record.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created project record response.</returns>
    public async Task<BaseResponse<ProjectDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists)
        {
            return BaseResponse<ProjectDto>.Fail("User does not exist.");
        }

        NodaTime.LocalDate? startDate = null;
        if (!string.IsNullOrWhiteSpace(request.StartDate))
        {
            var startDateParsed = _clockService.TryParseDate(request.StartDate);
            if (startDateParsed == null || !startDateParsed.Success)
            {
                return BaseResponse<ProjectDto>.Fail("Start date must be in yyyy-MM-dd format.");
            }

            startDate = startDateParsed.Value;
        }

        NodaTime.LocalDate? endDate = null;
        if (!string.IsNullOrWhiteSpace(request.EndDate))
        {
            var endDateParsed = _clockService.TryParseDate(request.EndDate);
            if (endDateParsed == null || !endDateParsed.Success)
            {
                return BaseResponse<ProjectDto>.Fail("End date must be in yyyy-MM-dd format.");
            }

            endDate = endDateParsed.Value;
        }

        var project = new Project
        {
            UserId = request.UserId,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            StartDate = startDate,
            EndDate = endDate,
            Url = request.Url?.Trim(),
            TechStack = request.TechStack?.Trim()
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<ProjectDto>.Ok(new ProjectDto(project), $"Created project with id {project.Id}");
    }
}
