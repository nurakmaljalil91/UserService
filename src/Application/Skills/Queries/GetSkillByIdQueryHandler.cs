#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Skills.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Skills.Queries;

/// <summary>
/// Handles retrieval of a skill by identifier.
/// </summary>
public class GetSkillByIdQueryHandler : IRequestHandler<GetSkillByIdQuery, BaseResponse<SkillDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSkillByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetSkillByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the skill lookup request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The skill response.</returns>
    public async Task<BaseResponse<SkillDto>> Handle(GetSkillByIdQuery request, CancellationToken cancellationToken)
    {
        var skill = await _context.Skills
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (skill == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Skill), request.Id.ToString());
        }

        return BaseResponse<SkillDto>.Ok(new SkillDto(skill), "Skill retrieved.");
    }
}
