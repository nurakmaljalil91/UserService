#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Mediator;

namespace Application.Groups.Commands;

/// <summary>
/// Handles the deletion of a group by its identifier.
/// </summary>
public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteGroupCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public DeleteGroupCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the group deletion request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A confirmation message response.</returns>
    public async Task<BaseResponse<string>> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.Groups.FindAsync(new object[] { request.Id }, cancellationToken);
        if (group == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Group), request.Id.ToString());
        }

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<string>.Ok($"Group with id {request.Id} deleted successfully.");
    }
}
