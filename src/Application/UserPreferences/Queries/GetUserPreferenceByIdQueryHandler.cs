#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.UserPreferences.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.UserPreferences.Queries;

/// <summary>
/// Handles retrieval of a user preference by identifier.
/// </summary>
public class GetUserPreferenceByIdQueryHandler : IRequestHandler<GetUserPreferenceByIdQuery, BaseResponse<UserPreferenceDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserPreferenceByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetUserPreferenceByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the user preference lookup request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user preference response.</returns>
    public async Task<BaseResponse<UserPreferenceDto>> Handle(GetUserPreferenceByIdQuery request, CancellationToken cancellationToken)
    {
        var preference = await _context.UserPreferences
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (preference == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.UserPreference), request.Id.ToString());
        }

        return BaseResponse<UserPreferenceDto>.Ok(new UserPreferenceDto(preference), "User preference retrieved.");
    }
}
