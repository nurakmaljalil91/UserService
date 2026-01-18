#nullable enable
using FluentValidation;

namespace Application.Sessions.Commands;

/// <summary>
/// Validates the <see cref="UpdateSessionCommand"/>.
/// </summary>
public class UpdateSessionCommandValidator : AbstractValidator<UpdateSessionCommand>
{
    private const int MaxRefreshTokenLength = 512;
    private const int MaxIpAddressLength = 64;
    private const int MaxUserAgentLength = 512;
    private const int MaxDeviceNameLength = 100;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSessionCommandValidator"/> class.
    /// </summary>
    public UpdateSessionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Session id is required.");

        RuleFor(x => x.RefreshToken)
            .MaximumLength(MaxRefreshTokenLength).When(x => x.RefreshToken != null)
            .WithMessage($"Refresh token must not exceed {MaxRefreshTokenLength} characters.");

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
