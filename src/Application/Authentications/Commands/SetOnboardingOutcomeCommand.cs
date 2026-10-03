#nullable enable
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Authentications.Commands;

/// <summary>
/// Records the authenticated user's profile onboarding choice.
/// </summary>
public sealed class SetOnboardingOutcomeCommand : IRequest<BaseResponse<string>>
{
    /// <summary>Gets or sets the desired outcome: Completed or Skipped.</summary>
    public string? Outcome { get; set; }
}

/// <summary>
/// Handles the current user's profile onboarding outcome.
/// </summary>
public sealed class SetOnboardingOutcomeCommandHandler : IRequestHandler<SetOnboardingOutcomeCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;

    /// <summary>Initializes the command handler.</summary>
    public SetOnboardingOutcomeCommandHandler(IApplicationDbContext context, IUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    /// <summary>Records Completed or Skipped for the current account.</summary>
    public async Task<BaseResponse<string>> Handle(SetOnboardingOutcomeCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<UserOnboardingStatus>(request.Outcome, true, out var outcome) ||
            outcome is not (UserOnboardingStatus.Completed or UserOnboardingStatus.Skipped))
        {
            return BaseResponse<string>.Fail("Outcome must be Completed or Skipped.");
        }

        if (_currentUser.UserId is not { } userId)
        {
            throw new UnauthorizedAccessException();
        }

        var user = await _context.Users.FirstOrDefaultAsync(
            candidate => candidate.Id == userId && !candidate.IsDeleted,
            cancellationToken);

        if (user == null)
        {
            return BaseResponse<string>.Fail("User was not found.");
        }

        if (user.OnboardingStatus == outcome)
        {
            return BaseResponse<string>.Ok(outcome.ToString(), "Onboarding outcome already recorded.");
        }

        if (user.OnboardingStatus != UserOnboardingStatus.Prompted)
        {
            return BaseResponse<string>.Fail("Onboarding outcome cannot be changed.");
        }

        if (await _context.TrySetOnboardingOutcomeAsync(userId, outcome, cancellationToken))
        {
            return BaseResponse<string>.Ok(outcome.ToString(), "Onboarding outcome recorded.");
        }

        var current = await _context.Users.AsNoTracking()
            .Where(candidate => candidate.Id == userId)
            .Select(candidate => candidate.OnboardingStatus)
            .SingleAsync(cancellationToken);
        return current == outcome
            ? BaseResponse<string>.Ok(outcome.ToString(), "Onboarding outcome already recorded.")
            : BaseResponse<string>.Fail("Onboarding outcome cannot be changed.");
    }
}
