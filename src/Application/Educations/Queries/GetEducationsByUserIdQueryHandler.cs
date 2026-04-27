#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Educations.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Educations.Queries;

/// <summary>
/// Handles retrieval of all education records for a specific user.
/// </summary>
public class GetEducationsByUserIdQueryHandler : IRequestHandler<GetEducationsByUserIdQuery, BaseResponse<IEnumerable<EducationDto>>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEducationsByUserIdQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetEducationsByUserIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the education records lookup request for a user.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of education records for the specified user.</returns>
    public async Task<BaseResponse<IEnumerable<EducationDto>>> Handle(GetEducationsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var educations = await _context.Educations
            .Where(e => e.UserId == request.UserId)
            .Select(e => new EducationDto(e))
            .ToListAsync(cancellationToken);

        return BaseResponse<IEnumerable<EducationDto>>.Ok(educations, $"Successfully retrieved {educations.Count} education records.");
    }
}
