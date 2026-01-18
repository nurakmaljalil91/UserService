#nullable enable
using Application.Common.Interfaces;
using Application.UserProfiles.Models;
using Domain.Common;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.UserProfiles.Commands;

/// <summary>
/// Handles creation of a new user profile.
/// </summary>
public class CreateUserProfileCommandHandler : IRequestHandler<CreateUserProfileCommand, BaseResponse<UserProfileDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IClockService _clockService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateUserProfileCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="clockService">The clock service for parsing dates.</param>
    public CreateUserProfileCommandHandler(IApplicationDbContext context, IClockService clockService)
    {
        _context = context;
        _clockService = clockService;
    }

    /// <summary>
    /// Handles the creation of a new user profile.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created user profile response.</returns>
    public async Task<BaseResponse<UserProfileDto>> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists)
        {
            return BaseResponse<UserProfileDto>.Fail("User does not exist.");
        }

        var profileExists = await _context.UserProfiles.AnyAsync(p => p.UserId == request.UserId, cancellationToken);
        if (profileExists)
        {
            return BaseResponse<UserProfileDto>.Fail("User profile already exists.");
        }

        var dateOfBirth = TryParseDateOfBirth(request.DateOfBirth, out var parseError);
        if (parseError != null)
        {
            return BaseResponse<UserProfileDto>.Fail(parseError);
        }

        var profile = new UserProfile
        {
            UserId = request.UserId,
            DisplayName = request.DisplayName?.Trim(),
            FirstName = request.FirstName?.Trim(),
            LastName = request.LastName?.Trim(),
            IdentityCardNumber = request.IdentityCardNumber?.Trim(),
            PassportNumber = request.PassportNumber?.Trim(),
            DateOfBirth = dateOfBirth,
            BirthPlace = request.BirthPlace?.Trim(),
            ShoeSize = request.ShoeSize?.Trim(),
            ClothingSize = request.ClothingSize?.Trim(),
            WaistSize = request.WaistSize?.Trim(),
            Bio = request.Bio?.Trim(),
            ImageUrl = request.ImageUrl?.Trim(),
            Tag = request.Tag?.Trim(),
            BloodType = request.BloodType?.Trim()
        };

        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<UserProfileDto>.Ok(new UserProfileDto(profile), $"Created user profile with id {profile.Id}");
    }

    private LocalDate? TryParseDateOfBirth(string? dateOfBirth, out string? error)
    {
        error = null;

        if (string.IsNullOrWhiteSpace(dateOfBirth))
        {
            return null;
        }

        var parsed = _clockService.TryParseDate(dateOfBirth);
        if (parsed == null || !parsed.Success)
        {
            error = "Date of birth must be in yyyy-MM-dd format.";
            return null;
        }

        return parsed.Value;
    }
}
