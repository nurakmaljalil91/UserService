#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Mediator;

namespace Application.Projects.Commands;

/// <summary>
/// Handles the deletion of a project by its identifier.
/// </summary>
public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteProjectCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public DeleteProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the project deletion request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A confirmation message response.</returns>
    public async Task<BaseResponse<string>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects.FindAsync(new object[] { request.Id }, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Project), request.Id.ToString());
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<string>.Ok($"Project with id {request.Id} deleted successfully.");
    }
}
