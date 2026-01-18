#nullable enable
using FluentValidation;

namespace Application.Sessions.Commands;

/// <summary>
/// Validates the <see cref="CreateSessionCommand"/>.
/// </summary>
public class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
{
    private const int MaxRefreshTokenLength = 512;
    private const int MaxIpAddressLength = 64;
    private const int MaxUserAgentLength = 512;
    private const int MaxDeviceNameLength = 100;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSessionCommandValidator"/> class.
    /// </summary>
    public CreateSessionCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.")
            .MaximumLength(MaxRefreshTokenLength).WithMessage($"Refresh token must not exceed {MaxRefreshTokenLength} characters.");

        RuleFor(x => x.ExpiresAt)
            .NotEmpty().WithMessage("ExpiresAt is required.");

        RuleFor(x => x.IpAddress)
            .MaximumLength(MaxIpAddressLength).When(x => x.IpAddress != null)
            .WithMessage($"IP address must not exceed {MaxIpAddressLength} characters.");

        RuleFor(x => x.UserAgent)
            .MaximumLength(MaxUserAgentLength).When(x => x.UserAgent != null)
            .WithMessage($"User agent must not exceed {MaxUserAgentLength} characters.");

        RuleFor(x => x.DeviceName)
            .MaximumLength(MaxDeviceNameLength).When(x => x.DeviceName != null)
            .WithMessage($"Device name must not exceed {MaxDeviceNameLength} characters.");
    }
}
