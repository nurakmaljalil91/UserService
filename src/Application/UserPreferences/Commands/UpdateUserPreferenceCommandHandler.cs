#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.UserPreferences.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.UserPreferences.Commands;

/// <summary>
/// Handles updating a user preference.
/// </summary>
public class UpdateUserPreferenceCommandHandler : IRequestHandler<UpdateUserPreferenceCommand, BaseResponse<UserPreferenceDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserPreferenceCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UpdateUserPreferenceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the user preference update request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated user preference response.</returns>
    public async Task<BaseResponse<UserPreferenceDto>> Handle(UpdateUserPreferenceCommand request, CancellationToken cancellationToken)
    {
        var preference = await _context.UserPreferences.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (preference == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.UserPreference), request.Id.ToString());
        }

        if (request.Key != null)
        {
            var key = request.Key.Trim();
            if (string.IsNullOrWhiteSpace(key))
            {
                return BaseResponse<UserPreferenceDto>.Fail("Preference key is required.");
            }

            var exists = await _context.UserPreferences.AnyAsync(
                p => p.Id != preference.Id && p.UserId == preference.UserId && p.Key == key,
                cancellationToken);

            if (exists)
            {
                return BaseResponse<UserPreferenceDto>.Fail("User preference key already exists.");
            }

            preference.Key = key;
        }

        if (request.Value != null)
        {
            preference.Value = request.Value.Trim();
        }

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<UserPreferenceDto>.Ok(new UserPreferenceDto(preference), "User preference updated.");
    }
}
