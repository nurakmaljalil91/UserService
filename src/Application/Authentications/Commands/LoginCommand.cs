#nullable enable
using Application.Authentications.Models;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Authentications.Commands;

/// <summary>
/// Command to authenticate a user.
/// </summary>
public class LoginCommand : IRequest<BaseResponse<LoginResponse>>
{
    /// <summary>
    /// Gets or sets the username for login.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the email for login.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the password for login.
    /// </summary>
    public string? Password { get; set; }
}

/// <summary>
/// Handles authentication of a user.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, BaseResponse<LoginResponse>>
{
    private const int DefaultExpiryMinutes = 60;
    private const string DefaultRole = "User";
    private const string InvalidCredentialsMessage = "Invalid username or password.";

    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IDateTime _dateTime;
    private readonly IClockService _clockService;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="passwordHasher">The password hashing service.</param>
    /// <param name="dateTime">The date and time service.</param>
    /// <param name="clockService">The clock service for NodaTime instants.</param>
    /// <param name="configuration">The application configuration.</param>
    public LoginCommandHandler(
        IApplicationDbContext context,
        IPasswordHasherService passwordHasher,
        IDateTime dateTime,
        IClockService clockService,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _dateTime = dateTime;
        _clockService = clockService;
        _configuration = configuration;
    }

    /// <summary>
    /// Handles authentication of a user.
    /// </summary>
    /// <param name="request">The login command.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A response indicating the outcome.</returns>
    public async Task<BaseResponse<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var identifier = !string.IsNullOrWhiteSpace(request.Username)
            ? request.Username
            : request.Email;

        if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BaseResponse<LoginResponse>.Fail(InvalidCredentialsMessage);
        }

        var normalizedIdentifier = identifier.Trim().ToUpperInvariant();

        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(
                u => u.NormalizedUsername == normalizedIdentifier || u.NormalizedEmail == normalizedIdentifier,
                cancellationToken);

        if (user == null || user.IsDeleted || user.IsLocked)
        {
            await RecordLoginAttemptAsync(null, identifier, false, "User not found or locked.");
            await _context.SaveChangesAsync(cancellationToken);
            return BaseResponse<LoginResponse>.Fail(InvalidCredentialsMessage);
        }

        if (string.IsNullOrWhiteSpace(user.PasswordHash) ||
            !_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password))
        {
            user.AccessFailedCount += 1;
            await RecordLoginAttemptAsync(user.Id, identifier, false, "Invalid password.");
            await _context.SaveChangesAsync(cancellationToken);
            return BaseResponse<LoginResponse>.Fail(InvalidCredentialsMessage);
        }

        user.AccessFailedCount = 0;

        var jwtSection = _configuration.GetSection("Jwt");
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var key = jwtSection["Key"];

        if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience) || string.IsNullOrWhiteSpace(key))
        {
            await RecordLoginAttemptAsync(user.Id, identifier, false, "JWT configuration is missing.");
            await _context.SaveChangesAsync(cancellationToken);
            return BaseResponse<LoginResponse>.Fail("JWT configuration is missing.");
        }

        var expiryMinutes = DefaultExpiryMinutes;
        if (int.TryParse(jwtSection["ExpiryMinutes"], out var configuredMinutes) && configuredMinutes > 0)
        {
            expiryMinutes = configuredMinutes;
        }

        var identity = string.IsNullOrWhiteSpace(user.Username) ? user.Email : user.Username;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, identity ?? user.Id.ToString())
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        var roles = user.UserRoles
            .Select(ur => ur.Role?.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (roles.Count == 0)
        {
            roles.Add(DefaultRole);
        }

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role!));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expires = _dateTime.UtcNow.AddMinutes(expiryMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        await RecordLoginAttemptAsync(user.Id, identifier, true, null);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<LoginResponse>.Ok(new LoginResponse(tokenValue, expires), "Login successful.");
    }

    private Task RecordLoginAttemptAsync(
        Guid? userId,
        string? identifier,
        bool isSuccessful,
        string? failureReason)
    {
        _context.LoginAttempts.Add(new LoginAttempt
        {
            UserId = userId,
            Identifier = identifier,
            IsSuccessful = isSuccessful,
            FailureReason = failureReason,
            AttemptedAt = _clockService.Now
        });

        return Task.CompletedTask;
    }
}

/// <summary>
/// Validates the <see cref="LoginCommand"/>.
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginCommandValidator"/> class.
    /// </summary>
    public LoginCommandValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Username) || !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Username or email is required.");
    }
}
