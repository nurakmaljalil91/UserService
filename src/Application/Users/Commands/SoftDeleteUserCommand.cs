#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands;

/// <summary>
/// Command to soft delete a user.
/// </summary>
public class SoftDeleteUserCommand : IRequest<BaseResponse<string>>
{
    public Guid Id { get; set; }
}

/// <summary>
/// Handles soft deletion of a user.
/// </summary>
public class SoftDeleteUserCommandHandler : IRequestHandler<SoftDeleteUserCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public SoftDeleteUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<string>> Handle(SoftDeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.User), request.Id.ToString());
        }

        user.IsDeleted = true;

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<string>.Ok("User deleted.");
    }
}
