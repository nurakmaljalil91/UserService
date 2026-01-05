#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Mediator;

namespace Application.Roles.Commands;

/// <summary>
/// Command to delete a role by its identifier.
/// </summary>
public class DeleteRoleCommand : IRequest<BaseResponse<string>>
{
    public Guid Id { get; set; }
}

/// <summary>
/// Handles the deletion of a role by its identifier.
/// </summary>
public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public DeleteRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<string>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _context.Roles.FindAsync(new object[] { request.Id }, cancellationToken);
        if (role == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Role), request.Id.ToString());
        }

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<string>.Ok($"Role with id {request.Id} deleted successfully.");
    }
}
