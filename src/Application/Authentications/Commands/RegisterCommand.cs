#nullable enable
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Notification.Contracts.Events;
using Notification.Contracts.Models;

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
    private const string DefaultUserRoleName = "User";
    private const string DefaultUserRoleNormalizedName = "USER";
    private const string SourceService = "UserService";
    private const string UserRegisteredEventType = "UserRegistered";

    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly INotificationRequestPublisher _notificationPublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="passwordHasher">The password hashing service.</param>
    /// <param name="notificationPublisher">The publisher for notification requests.</param>
    public RegisterCommandHandler(
        IApplicationDbContext context,
        IPasswordHasherService passwordHasher,
        INotificationRequestPublisher notificationPublisher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _notificationPublisher = notificationPublisher;
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
            Id = Guid.NewGuid(),
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

        var userRole = await _context.Roles.SingleOrDefaultAsync(
            role => role.NormalizedName == DefaultUserRoleNormalizedName,
            cancellationToken);

        if (userRole == null)
        {
            userRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = DefaultUserRoleName,
                NormalizedName = DefaultUserRoleNormalizedName,
                Description = "Standard user"
            };

            _context.Roles.Add(userRole);
        }

        _context.Users.Add(user);
        _context.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = userRole.Id
        });

        await _context.SaveChangesAsync(cancellationToken);

        await _notificationPublisher.PublishAsync(CreateWelcomeNotification(user, UserRegisteredEventType, user.Id.ToString()), cancellationToken);

        return BaseResponse<string>.Ok(user.Id.ToString(), "User registered.");
    }

    private static NotificationRequestedV1 CreateWelcomeNotification(
        User user,
        string sourceEventType,
        string sourceEventId)
    {
        var message = $"Welcome {user.Username}!";

        return new NotificationRequestedV1
        {
            SourceService = SourceService,
            SourceEventType = sourceEventType,
            SourceEventId = sourceEventId,
            Title = message,
            Body = message,
            Priority = NotificationPriorityV1.Normal,
            Channels = new[] { NotificationChannelV1.InApp },
            Recipients = new[]
            {
                new NotificationRecipientV1
                {
                    RecipientId = user.Id.ToString(),
                    RecipientType = RecipientTypeV1.Individual,
                    DisplayName = user.Username,
                    Email = user.Email,
                    InAppEnabled = true,
                    Channels = new[] { NotificationChannelV1.InApp }
                }
            }
        };
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
