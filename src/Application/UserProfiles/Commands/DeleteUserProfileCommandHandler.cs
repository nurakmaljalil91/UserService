#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Mediator;

namespace Application.UserProfiles.Commands;

/// <summary>
/// Handles the deletion of a user profile by its identifier.
/// </summary>
public class DeleteUserProfileCommandHandler : IRequestHandler<DeleteUserProfileCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteUserProfileCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public DeleteUserProfileCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the user profile deletion request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A confirmation message response.</returns>
    public async Task<BaseResponse<string>> Handle(DeleteUserProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _context.UserProfiles.FindAsync(new object[] { request.Id }, cancellationToken);
        if (profile == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.UserProfile), request.Id.ToString());
        }

        _context.UserProfiles.Remove(profile);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<string>.Ok($"User profile with id {request.Id} deleted successfully.");
    }
}
