#nullable enable
using Application.Common.Interfaces;
using Domain.Common;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Authentications.Commands;

/// <summary>
/// Command to reset a user's password.
/// </summary>
public class ResetPasswordCommand : IRequest<BaseResponse<string>>
{
    /// <summary>
    /// Gets or sets the email for the user.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the reset token.
    /// </summary>
    public string? ResetToken { get; set; }

    /// <summary>
    /// Gets or sets the new password.
    /// </summary>
    public string? NewPassword { get; set; }
}

/// <summary>
/// Handles resetting a user's password.
/// </summary>
public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IClockService _clockService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResetPasswordCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="passwordHasher">The password hashing service.</param>
    /// <param name="clockService">The clock service.</param>
    public ResetPasswordCommandHandler(
        IApplicationDbContext context,
        IPasswordHasherService passwordHasher,
        IClockService clockService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _clockService = clockService;
    }

    /// <summary>
    /// Handles resetting a user's password.
    /// </summary>
    /// <param name="request">The reset password command.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A response indicating the outcome.</returns>
    public async Task<BaseResponse<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email?.Trim();
        if (string.IsNullOrWhiteSpace(email))
        {
            return BaseResponse<string>.Fail("Email is required.");
        }

        var normalizedEmail = email.ToUpperInvariant();

        var user = await _context.Users.FirstOrDefaultAsync(
            u => u.NormalizedEmail == normalizedEmail,
            cancellationToken);

        if (user == null || user.IsDeleted)
        {
            return BaseResponse<string>.Fail("User not found.");
        }

        if (string.IsNullOrWhiteSpace(user.PasswordResetToken) ||
            !string.Equals(user.PasswordResetToken, request.ResetToken, StringComparison.Ordinal))
        {
            return BaseResponse<string>.Fail("Reset token is invalid.");
        }

        if (user.PasswordResetTokenExpiresAt.HasValue &&
            user.PasswordResetTokenExpiresAt.Value <= _clockService.Now)
        {
            return BaseResponse<string>.Fail("Reset token has expired.");
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword ?? string.Empty);
        user.AccessFailedCount = 0;
        user.IsLocked = false;
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiresAt = null;

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<string>.Ok("Password reset successful.");
    }
}

/// <summary>
/// Validates the <see cref="ResetPasswordCommand"/>.
/// </summary>
public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResetPasswordCommandValidator"/> class.
    /// </summary>
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is invalid.");

        RuleFor(x => x.ResetToken)
            .NotEmpty().WithMessage("Reset token is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.");
    }
}
