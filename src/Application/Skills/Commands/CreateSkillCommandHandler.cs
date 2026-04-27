#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Skills.Models;
using Domain.Common;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Skills.Commands;

/// <summary>
/// Handles creation of a new skill.
/// </summary>
public class CreateSkillCommandHandler : IRequestHandler<CreateSkillCommand, BaseResponse<SkillDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSkillCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public CreateSkillCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the creation of a new skill.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created skill response.</returns>
    public async Task<BaseResponse<SkillDto>> Handle(CreateSkillCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists)
        {
            return BaseResponse<SkillDto>.Fail("User does not exist.");
        }

        var skill = new Skill
        {
            UserId = request.UserId,
            Name = request.Name.Trim(),
            Proficiency = request.Proficiency?.Trim(),
            YearsOfExperience = request.YearsOfExperience
        };

        _context.Skills.Add(skill);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<SkillDto>.Ok(new SkillDto(skill), $"Created skill with id {skill.Id}");
    }
}
