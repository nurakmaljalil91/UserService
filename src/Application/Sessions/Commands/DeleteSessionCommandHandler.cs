#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Mediator;

namespace Application.Sessions.Commands;

/// <summary>
/// Handles the deletion of a session by its identifier.
/// </summary>
public class DeleteSessionCommandHandler : IRequestHandler<DeleteSessionCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSessionCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public DeleteSessionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the session deletion request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A confirmation message response.</returns>
    public async Task<BaseResponse<string>> Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.Sessions.FindAsync(new object[] { request.Id }, cancellationToken);
        if (session == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Session), request.Id.ToString());
        }

        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<string>.Ok($"Session with id {request.Id} deleted successfully.");
    }
}
