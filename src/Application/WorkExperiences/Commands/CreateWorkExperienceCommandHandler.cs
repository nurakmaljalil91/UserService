#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.WorkExperiences.Models;
using Domain.Common;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.WorkExperiences.Commands;

/// <summary>
/// Handles creation of a new work experience record.
/// </summary>
public class CreateWorkExperienceCommandHandler : IRequestHandler<CreateWorkExperienceCommand, BaseResponse<WorkExperienceDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IClockService _clockService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateWorkExperienceCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="clockService">The clock service for parsing dates.</param>
    public CreateWorkExperienceCommandHandler(IApplicationDbContext context, IClockService clockService)
    {
        _context = context;
        _clockService = clockService;
    }

    /// <summary>
    /// Handles the creation of a new work experience record.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created work experience record response.</returns>
    public async Task<BaseResponse<WorkExperienceDto>> Handle(CreateWorkExperienceCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists)
        {
            return BaseResponse<WorkExperienceDto>.Fail("User does not exist.");
        }

        var startDateParsed = _clockService.TryParseDate(request.StartDate);
        if (startDateParsed == null || !startDateParsed.Success)
        {
            return BaseResponse<WorkExperienceDto>.Fail("Start date must be in yyyy-MM-dd format.");
        }

        NodaTime.LocalDate? endDate = null;
        if (!string.IsNullOrWhiteSpace(request.EndDate))
        {
            var endDateParsed = _clockService.TryParseDate(request.EndDate);
            if (endDateParsed == null || !endDateParsed.Success)
            {
                return BaseResponse<WorkExperienceDto>.Fail("End date must be in yyyy-MM-dd format.");
            }

            endDate = endDateParsed.Value;
        }

        var workExperience = new WorkExperience
        {
            UserId = request.UserId,
            Company = request.Company.Trim(),
            Position = request.Position.Trim(),
            StartDate = startDateParsed.Value,
            EndDate = endDate,
            Description = request.Description?.Trim(),
            Location = request.Location?.Trim()
        };

        _context.WorkExperiences.Add(workExperience);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<WorkExperienceDto>.Ok(new WorkExperienceDto(workExperience), $"Created work experience record with id {workExperience.Id}");
    }
}
