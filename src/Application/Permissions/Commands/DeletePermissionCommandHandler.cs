#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Mediator;

namespace Application.Permissions.Commands;

/// <summary>
/// Handles the deletion of a permission by its identifier.
/// </summary>
public class DeletePermissionCommandHandler : IRequestHandler<DeletePermissionCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeletePermissionCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public DeletePermissionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the permission deletion request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A confirmation message response.</returns>
    public async Task<BaseResponse<string>> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await _context.Permissions.FindAsync(new object[] { request.Id }, cancellationToken);
        if (permission == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Permission), request.Id.ToString());
        }

        _context.Permissions.Remove(permission);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<string>.Ok($"Permission with id {request.Id} deleted successfully.");
    }
}
