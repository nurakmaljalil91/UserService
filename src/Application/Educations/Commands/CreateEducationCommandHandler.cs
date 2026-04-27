#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Educations.Models;
using Domain.Common;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Educations.Commands;

/// <summary>
/// Handles creation of a new education record.
/// </summary>
public class CreateEducationCommandHandler : IRequestHandler<CreateEducationCommand, BaseResponse<EducationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IClockService _clockService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateEducationCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="clockService">The clock service for parsing dates.</param>
    public CreateEducationCommandHandler(IApplicationDbContext context, IClockService clockService)
    {
        _context = context;
        _clockService = clockService;
    }

    /// <summary>
    /// Handles the creation of a new education record.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created education record response.</returns>
    public async Task<BaseResponse<EducationDto>> Handle(CreateEducationCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists)
        {
            return BaseResponse<EducationDto>.Fail("User does not exist.");
        }

        var startDateParsed = _clockService.TryParseDate(request.StartDate);
        if (startDateParsed == null || !startDateParsed.Success)
        {
            return BaseResponse<EducationDto>.Fail("Start date must be in yyyy-MM-dd format.");
        }

        NodaTime.LocalDate? endDate = null;
        if (!string.IsNullOrWhiteSpace(request.EndDate))
        {
            var endDateParsed = _clockService.TryParseDate(request.EndDate);
            if (endDateParsed == null || !endDateParsed.Success)
            {
                return BaseResponse<EducationDto>.Fail("End date must be in yyyy-MM-dd format.");
            }

            endDate = endDateParsed.Value;
        }

        var education = new Education
        {
            UserId = request.UserId,
            Institution = request.Institution.Trim(),
            Degree = request.Degree?.Trim(),
            FieldOfStudy = request.FieldOfStudy?.Trim(),
            StartDate = startDateParsed.Value,
            EndDate = endDate,
            Grade = request.Grade?.Trim(),
            Description = request.Description?.Trim()
        };

        _context.Educations.Add(education);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<EducationDto>.Ok(new EducationDto(education), $"Created education record with id {education.Id}");
    }
}
