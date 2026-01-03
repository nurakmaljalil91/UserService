#nullable enable
using Application.Common.Interfaces;
using Application.Users.Models;
using Domain.Common;
using Domain.Entities;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands;

/// <summary>
/// Command to create a new user.
/// </summary>
public class CreateUserCommand : IRequest<BaseResponse<UserDto>>
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Password { get; set; }
}

/// <summary>
/// Handles creation of a new user.
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, BaseResponse<UserDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasher;

    public CreateUserCommandHandler(IApplicationDbContext context, IPasswordHasherService passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<BaseResponse<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var username = request.Username?.Trim();
        var email = request.Email?.Trim();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
        {
            return BaseResponse<UserDto>.Fail("Username and email are required.");
        }

        var normalizedUsername = username.ToUpperInvariant();
        var normalizedEmail = email.ToUpperInvariant();

        var exists = await _context.Users.AnyAsync(
            u => u.NormalizedUsername == normalizedUsername || u.NormalizedEmail == normalizedEmail,
            cancellationToken);

        if (exists)
        {
            return BaseResponse<UserDto>.Fail("Username or email already exists.");
        }

        var user = new User
        {
            Username = username,
            NormalizedUsername = normalizedUsername,
            Email = email,
            NormalizedEmail = normalizedEmail,
            PhoneNumber = request.PhoneNumber?.Trim(),
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

        return BaseResponse<UserDto>.Ok(new UserDto(user), $"Created user with id {user.Id}");
    }
}

/// <summary>
/// Validates the <see cref="CreateUserCommand"/>.
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
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
