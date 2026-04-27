#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.WorkExperiences.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.WorkExperiences.Queries;

/// <summary>
/// Handles retrieval of all work experience records for a specific user.
/// </summary>
public class GetWorkExperiencesByUserIdQueryHandler : IRequestHandler<GetWorkExperiencesByUserIdQuery, BaseResponse<IEnumerable<WorkExperienceDto>>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetWorkExperiencesByUserIdQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetWorkExperiencesByUserIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the work experience records lookup request for a user.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of work experience records for the specified user.</returns>
    public async Task<BaseResponse<IEnumerable<WorkExperienceDto>>> Handle(GetWorkExperiencesByUserIdQuery request, CancellationToken cancellationToken)
    {
        var workExperiences = await _context.WorkExperiences
            .Where(w => w.UserId == request.UserId)
            .Select(w => new WorkExperienceDto(w))
            .ToListAsync(cancellationToken);

        return BaseResponse<IEnumerable<WorkExperienceDto>>.Ok(workExperiences, $"Successfully retrieved {workExperiences.Count} work experience records.");
    }
}
