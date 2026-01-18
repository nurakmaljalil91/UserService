#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.UserProfiles.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.UserProfiles.Queries;

/// <summary>
/// Handles retrieval of a user profile by identifier.
/// </summary>
public class GetUserProfileByIdQueryHandler : IRequestHandler<GetUserProfileByIdQuery, BaseResponse<UserProfileDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserProfileByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetUserProfileByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the user profile lookup request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user profile response.</returns>
    public async Task<BaseResponse<UserProfileDto>> Handle(GetUserProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var profile = await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (profile == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.UserProfile), request.Id.ToString());
        }

        return BaseResponse<UserProfileDto>.Ok(new UserProfileDto(profile), "User profile retrieved.");
    }
}
