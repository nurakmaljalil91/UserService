#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Skills.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Skills.Queries;

/// <summary>
/// Handles retrieval of all skills for a specific user.
/// </summary>
public class GetSkillsByUserIdQueryHandler : IRequestHandler<GetSkillsByUserIdQuery, BaseResponse<IEnumerable<SkillDto>>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSkillsByUserIdQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetSkillsByUserIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the skills lookup request for a user.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of skills for the specified user.</returns>
    public async Task<BaseResponse<IEnumerable<SkillDto>>> Handle(GetSkillsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var skills = await _context.Skills
            .Where(s => s.UserId == request.UserId)
            .Select(s => new SkillDto(s))
            .ToListAsync(cancellationToken);

        return BaseResponse<IEnumerable<SkillDto>>.Ok(skills, $"Successfully retrieved {skills.Count} skills.");
    }
}
