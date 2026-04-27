#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Mediator;

namespace Application.Skills.Commands;

/// <summary>
/// Handles the deletion of a skill by its identifier.
/// </summary>
public class DeleteSkillCommandHandler : IRequestHandler<DeleteSkillCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSkillCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public DeleteSkillCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the skill deletion request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A confirmation message response.</returns>
    public async Task<BaseResponse<string>> Handle(DeleteSkillCommand request, CancellationToken cancellationToken)
    {
        var skill = await _context.Skills.FindAsync(new object[] { request.Id }, cancellationToken);
        if (skill == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Skill), request.Id.ToString());
        }

        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<string>.Ok($"Skill with id {request.Id} deleted successfully.");
    }
}
