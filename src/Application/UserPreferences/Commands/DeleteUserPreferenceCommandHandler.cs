#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Mediator;

namespace Application.UserPreferences.Commands;

/// <summary>
/// Handles the deletion of a user preference by its identifier.
/// </summary>
public class DeleteUserPreferenceCommandHandler : IRequestHandler<DeleteUserPreferenceCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteUserPreferenceCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public DeleteUserPreferenceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the user preference deletion request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A confirmation message response.</returns>
    public async Task<BaseResponse<string>> Handle(DeleteUserPreferenceCommand request, CancellationToken cancellationToken)
    {
        var preference = await _context.UserPreferences.FindAsync(new object[] { request.Id }, cancellationToken);
        if (preference == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.UserPreference), request.Id.ToString());
        }

        _context.UserPreferences.Remove(preference);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<string>.Ok($"User preference with id {request.Id} deleted successfully.");
    }
}
