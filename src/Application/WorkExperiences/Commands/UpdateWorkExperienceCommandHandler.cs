#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.WorkExperiences.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.WorkExperiences.Commands;

/// <summary>
/// Handles updating an existing work experience record.
/// </summary>
public class UpdateWorkExperienceCommandHandler : IRequestHandler<UpdateWorkExperienceCommand, BaseResponse<WorkExperienceDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IClockService _clockService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateWorkExperienceCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="clockService">The clock service for parsing dates.</param>
    public UpdateWorkExperienceCommandHandler(IApplicationDbContext context, IClockService clockService)
    {
        _context = context;
        _clockService = clockService;
    }

    /// <summary>
    /// Handles the work experience record update request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated work experience record response.</returns>
    public async Task<BaseResponse<WorkExperienceDto>> Handle(UpdateWorkExperienceCommand request, CancellationToken cancellationToken)
    {
        var workExperience = await _context.WorkExperiences
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (workExperience == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.WorkExperience), request.Id.ToString());
        }

        if (request.Company != null)
        {
            workExperience.Company = request.Company.Trim();
        }

        if (request.Position != null)
        {
            workExperience.Position = request.Position.Trim();
        }

        if (request.StartDate != null)
        {
            var parsed = _clockService.TryParseDate(request.StartDate);
            if (parsed == null || !parsed.Success)
            {
                return BaseResponse<WorkExperienceDto>.Fail("Start date must be in yyyy-MM-dd format.");
            }

            workExperience.StartDate = parsed.Value;
        }

        if (request.EndDate != null)
        {
            if (string.IsNullOrWhiteSpace(request.EndDate))
            {
                workExperience.EndDate = null;
            }
            else
            {
                var parsed = _clockService.TryParseDate(request.EndDate);
                if (parsed == null || !parsed.Success)
                {
                    return BaseResponse<WorkExperienceDto>.Fail("End date must be in yyyy-MM-dd format.");
                }

                workExperience.EndDate = parsed.Value;
            }
        }

        if (request.Description != null)
        {
            workExperience.Description = request.Description.Trim();
        }

        if (request.Location != null)
        {
            workExperience.Location = request.Location.Trim();
        }

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<WorkExperienceDto>.Ok(new WorkExperienceDto(workExperience), "Work experience record updated.");
    }
}
