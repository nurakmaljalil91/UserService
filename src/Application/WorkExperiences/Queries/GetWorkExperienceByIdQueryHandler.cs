#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.WorkExperiences.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.WorkExperiences.Queries;

/// <summary>
/// Handles retrieval of a work experience record by identifier.
/// </summary>
public class GetWorkExperienceByIdQueryHandler : IRequestHandler<GetWorkExperienceByIdQuery, BaseResponse<WorkExperienceDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetWorkExperienceByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetWorkExperienceByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the work experience record lookup request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The work experience record response.</returns>
    public async Task<BaseResponse<WorkExperienceDto>> Handle(GetWorkExperienceByIdQuery request, CancellationToken cancellationToken)
    {
        var workExperience = await _context.WorkExperiences
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (workExperience == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.WorkExperience), request.Id.ToString());
        }

        return BaseResponse<WorkExperienceDto>.Ok(new WorkExperienceDto(workExperience), "Work experience record retrieved.");
    }
}
