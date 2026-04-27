#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Skills.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Skills.Commands;

/// <summary>
/// Handles updating an existing skill.
/// </summary>
public class UpdateSkillCommandHandler : IRequestHandler<UpdateSkillCommand, BaseResponse<SkillDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSkillCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UpdateSkillCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the skill update request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated skill response.</returns>
    public async Task<BaseResponse<SkillDto>> Handle(UpdateSkillCommand request, CancellationToken cancellationToken)
    {
        var skill = await _context.Skills
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (skill == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Skill), request.Id.ToString());
        }

        if (request.Name != null)
        {
            skill.Name = request.Name.Trim();
        }

        if (request.Proficiency != null)
        {
            skill.Proficiency = request.Proficiency.Trim();
        }

        if (request.YearsOfExperience.HasValue)
        {
            skill.YearsOfExperience = request.YearsOfExperience;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<SkillDto>.Ok(new SkillDto(skill), "Skill updated.");
    }
}
