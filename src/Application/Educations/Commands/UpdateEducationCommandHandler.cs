#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Educations.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Educations.Commands;

/// <summary>
/// Handles updating an existing education record.
/// </summary>
public class UpdateEducationCommandHandler : IRequestHandler<UpdateEducationCommand, BaseResponse<EducationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IClockService _clockService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateEducationCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="clockService">The clock service for parsing dates.</param>
    public UpdateEducationCommandHandler(IApplicationDbContext context, IClockService clockService)
    {
        _context = context;
        _clockService = clockService;
    }

    /// <summary>
    /// Handles the education record update request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated education record response.</returns>
    public async Task<BaseResponse<EducationDto>> Handle(UpdateEducationCommand request, CancellationToken cancellationToken)
    {
        var education = await _context.Educations
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (education == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Education), request.Id.ToString());
        }

        if (request.Institution != null)
        {
            education.Institution = request.Institution.Trim();
        }

        if (request.Degree != null)
        {
            education.Degree = request.Degree.Trim();
        }

        if (request.FieldOfStudy != null)
        {
            education.FieldOfStudy = request.FieldOfStudy.Trim();
        }

        if (request.StartDate != null)
        {
            var parsed = _clockService.TryParseDate(request.StartDate);
            if (parsed == null || !parsed.Success)
            {
                return BaseResponse<EducationDto>.Fail("Start date must be in yyyy-MM-dd format.");
            }

            education.StartDate = parsed.Value;
        }

        if (request.EndDate != null)
        {
            if (string.IsNullOrWhiteSpace(request.EndDate))
            {
                education.EndDate = null;
            }
            else
            {
                var parsed = _clockService.TryParseDate(request.EndDate);
                if (parsed == null || !parsed.Success)
                {
                    return BaseResponse<EducationDto>.Fail("End date must be in yyyy-MM-dd format.");
                }

                education.EndDate = parsed.Value;
            }
        }

        if (request.Grade != null)
        {
            education.Grade = request.Grade.Trim();
        }

        if (request.Description != null)
        {
            education.Description = request.Description.Trim();
        }

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<EducationDto>.Ok(new EducationDto(education), "Education record updated.");
    }
}
