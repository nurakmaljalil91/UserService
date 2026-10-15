#nullable enable
using Application.Common.Interfaces;
using Domain.ValueObjects;
using FluentValidation;
using NodaTime;

namespace Application.PersonalAccessTokens.Commands;

/// <summary>
/// Validates <see cref="CreatePersonalAccessTokenCommand"/>.
/// </summary>
public class CreatePersonalAccessTokenCommandValidator : AbstractValidator<CreatePersonalAccessTokenCommand>
{
    private const int MaxNameLength = 100;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreatePersonalAccessTokenCommandValidator"/> class.
    /// </summary>
    /// <param name="clock">The clock used to reject expiry dates that are not in the future.</param>
    public CreatePersonalAccessTokenCommandValidator(IClockService clock)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(MaxNameLength).WithMessage($"Name must not exceed {MaxNameLength} characters.");

        RuleFor(x => x.Scopes)
            .NotEmpty().WithMessage("At least one scope is required.");

        RuleForEach(x => x.Scopes)
            .Must(TokenScope.IsValid)
            .WithMessage((_, scope) => $"Unsupported scope '{scope}'.");

        RuleFor(x => x.ExpiresAt)
            .Must(expiresAt => Instant.FromDateTimeOffset(expiresAt!.Value) > clock.Now)
            .When(x => x.ExpiresAt.HasValue)
            .WithMessage("ExpiresAt must be in the future.");
    }
}
