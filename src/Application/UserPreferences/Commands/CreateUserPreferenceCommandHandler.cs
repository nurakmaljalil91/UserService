#nullable enable
using Application.Common.Interfaces;
using Application.UserPreferences.Models;
using Domain.Common;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.UserPreferences.Commands;

/// <summary>
/// Handles creation of a new user preference.
/// </summary>
public class CreateUserPreferenceCommandHandler : IRequestHandler<CreateUserPreferenceCommand, BaseResponse<UserPreferenceDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateUserPreferenceCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public CreateUserPreferenceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the creation of a new user preference.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created user preference response.</returns>
    public async Task<BaseResponse<UserPreferenceDto>> Handle(CreateUserPreferenceCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists)
        {
            return BaseResponse<UserPreferenceDto>.Fail("User does not exist.");
        }

        var key = request.Key!.Trim();
        var exists = await _context.UserPreferences.AnyAsync(
            p => p.UserId == request.UserId && p.Key == key,
            cancellationToken);

        if (exists)
        {
            return BaseResponse<UserPreferenceDto>.Fail("User preference key already exists.");
        }

        var preference = new UserPreference
        {
            UserId = request.UserId,
            Key = key,
            Value = request.Value?.Trim()
        };

        _context.UserPreferences.Add(preference);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<UserPreferenceDto>.Ok(new UserPreferenceDto(preference), $"Created user preference with id {preference.Id}");
    }
}
