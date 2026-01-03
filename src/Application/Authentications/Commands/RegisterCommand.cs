#nullable enable
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Authentications.Commands;

/// <summary>
/// Command to register a new user.
/// </summary>
public class RegisterCommand : IRequest<BaseResponse<string>>
{
    /// <summary>
    /// Gets or sets the username for the new user.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the email for the new user.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the password for the new user.
    /// </summary>
    public string? Password { get; set; }
}

/// <summary>
/// Handles user registration.
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasher;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="passwordHasher">The password hashing service.</param>
    public RegisterCommandHandler(IApplicationDbContext context, IPasswordHasherService passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Handles user registration.
    /// </summary>
    /// <param name="request">The registration command.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A response indicating the outcome.</returns>
    public async Task<BaseResponse<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var username = request.Username?.Trim();
        var email = request.Email?.Trim();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
        {
            return BaseResponse<string>.Fail("Username and email are required.");
        }

        var normalizedUsername = username.ToUpperInvariant();
        var normalizedEmail = email.ToUpperInvariant();

        var exists = await _context.Users.AnyAsync(
            u => u.NormalizedUsername == normalizedUsername || u.NormalizedEmail == normalizedEmail,
            cancellationToken);

        if (exists)
        {
            return BaseResponse<string>.Fail("Username or email already exists.");
        }

        var user = new User
        {
            Username = username,
            NormalizedUsername = normalizedUsername,
            Email = email,
            NormalizedEmail = normalizedEmail,
            EmailConfirm = false,
            PhoneNumberConfirm = false,
            TwoFactorEnabled = false,
            AccessFailedCount = 0,
            IsLocked = false,
            IsDeleted = false
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password ?? string.Empty);

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<string>.Ok(user.Id.ToString(), "User registered.");
    }
}

/// <summary>
/// Validates the <see cref="RegisterCommand"/>.
/// </summary>
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterCommandValidator"/> class.
    /// </summary>
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is invalid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
