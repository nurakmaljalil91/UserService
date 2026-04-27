#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Mediator;

namespace Application.Educations.Commands;

/// <summary>
/// Handles the deletion of an education record by its identifier.
/// </summary>
public class DeleteEducationCommandHandler : IRequestHandler<DeleteEducationCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteEducationCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public DeleteEducationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the education record deletion request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A confirmation message response.</returns>
    public async Task<BaseResponse<string>> Handle(DeleteEducationCommand request, CancellationToken cancellationToken)
    {
        var education = await _context.Educations.FindAsync(new object[] { request.Id }, cancellationToken);
        if (education == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Education), request.Id.ToString());
        }

        _context.Educations.Remove(education);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<string>.Ok($"Education record with id {request.Id} deleted successfully.");
    }
}
