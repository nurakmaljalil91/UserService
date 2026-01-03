#nullable enable
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Users.Models;
using Domain.Common;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands;

/// <summary>
/// Command to update an existing user.
/// </summary>
public class UpdateUserCommand : IRequest<BaseResponse<UserDto>>
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsLocked { get; set; }
}

/// <summary>
/// Handles updating a user.
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, BaseResponse<UserDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.User), request.Id.ToString());
        }

        var username = request.Username?.Trim();
        var email = request.Email?.Trim();

        if (!string.IsNullOrWhiteSpace(username))
        {
            var normalizedUsername = username.ToUpperInvariant();
            var usernameExists = await _context.Users.AnyAsync(
                u => u.Id != user.Id && u.NormalizedUsername == normalizedUsername,
                cancellationToken);

            if (usernameExists)
            {
                return BaseResponse<UserDto>.Fail("Username already exists.");
            }

            user.Username = username;
            user.NormalizedUsername = normalizedUsername;
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            var normalizedEmail = email.ToUpperInvariant();
            var emailExists = await _context.Users.AnyAsync(
                u => u.Id != user.Id && u.NormalizedEmail == normalizedEmail,
                cancellationToken);

            if (emailExists)
            {
                return BaseResponse<UserDto>.Fail("Email already exists.");
            }

            user.Email = email;
            user.NormalizedEmail = normalizedEmail;
        }

        if (request.PhoneNumber != null)
        {
            user.PhoneNumber = request.PhoneNumber.Trim();
        }

        if (request.IsLocked.HasValue)
        {
            user.IsLocked = request.IsLocked.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<UserDto>.Ok(new UserDto(user), "User updated.");
    }
}

/// <summary>
/// Validates the <see cref="UpdateUserCommand"/>.
/// </summary>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Email is invalid.");
    }
}
