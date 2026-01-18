#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.UserProfiles.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.UserProfiles.Commands;

/// <summary>
/// Handles updating a user profile.
/// </summary>
public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, BaseResponse<UserProfileDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IClockService _clockService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserProfileCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="clockService">The clock service for parsing dates.</param>
    public UpdateUserProfileCommandHandler(IApplicationDbContext context, IClockService clockService)
    {
        _context = context;
        _clockService = clockService;
    }

    /// <summary>
    /// Handles the user profile update request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated user profile response.</returns>
    public async Task<BaseResponse<UserProfileDto>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (profile == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.UserProfile), request.Id.ToString());
        }

        if (request.DisplayName != null)
        {
            profile.DisplayName = request.DisplayName.Trim();
        }

        if (request.FirstName != null)
        {
            profile.FirstName = request.FirstName.Trim();
        }

        if (request.LastName != null)
        {
            profile.LastName = request.LastName.Trim();
        }

        if (request.IdentityCardNumber != null)
        {
            profile.IdentityCardNumber = request.IdentityCardNumber.Trim();
        }

        if (request.PassportNumber != null)
        {
            profile.PassportNumber = request.PassportNumber.Trim();
        }

        if (request.DateOfBirth != null)
        {
            if (string.IsNullOrWhiteSpace(request.DateOfBirth))
            {
                profile.DateOfBirth = null;
            }
            else
            {
                var parsed = _clockService.TryParseDate(request.DateOfBirth);
                if (parsed == null || !parsed.Success)
                {
                    return BaseResponse<UserProfileDto>.Fail("Date of birth must be in yyyy-MM-dd format.");
                }

                profile.DateOfBirth = parsed.Value;
            }
        }

        if (request.BirthPlace != null)
        {
            profile.BirthPlace = request.BirthPlace.Trim();
        }

        if (request.ShoeSize != null)
        {
            profile.ShoeSize = request.ShoeSize.Trim();
        }

        if (request.ClothingSize != null)
        {
            profile.ClothingSize = request.ClothingSize.Trim();
        }

        if (request.WaistSize != null)
        {
            profile.WaistSize = request.WaistSize.Trim();
        }

        if (request.Bio != null)
        {
            profile.Bio = request.Bio.Trim();
        }

        if (request.ImageUrl != null)
        {
            profile.ImageUrl = request.ImageUrl.Trim();
        }

        if (request.Tag != null)
        {
            profile.Tag = request.Tag.Trim();
        }

        if (request.BloodType != null)
        {
            profile.BloodType = request.BloodType.Trim();
        }

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<UserProfileDto>.Ok(new UserProfileDto(profile), "User profile updated.");
    }
}
