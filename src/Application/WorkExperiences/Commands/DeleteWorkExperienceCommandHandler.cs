#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Mediator;

namespace Application.WorkExperiences.Commands;

/// <summary>
/// Handles the deletion of a work experience record by its identifier.
/// </summary>
public class DeleteWorkExperienceCommandHandler : IRequestHandler<DeleteWorkExperienceCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteWorkExperienceCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public DeleteWorkExperienceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the work experience record deletion request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A confirmation message response.</returns>
    public async Task<BaseResponse<string>> Handle(DeleteWorkExperienceCommand request, CancellationToken cancellationToken)
    {
        var workExperience = await _context.WorkExperiences.FindAsync(new object[] { request.Id }, cancellationToken);
        if (workExperience == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.WorkExperience), request.Id.ToString());
        }

        _context.WorkExperiences.Remove(workExperience);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<string>.Ok($"Work experience record with id {request.Id} deleted successfully.");
    }
}
