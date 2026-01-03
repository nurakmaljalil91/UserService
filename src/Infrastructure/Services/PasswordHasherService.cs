#nullable enable
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

/// <summary>
/// Implements password hashing using ASP.NET Core Identity's <see cref="PasswordHasher{TUser}"/>.
/// </summary>
public sealed class PasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<User> _hasher = new();

    /// <inheritdoc />
    public string HashPassword(User user, string password)
    {
        return _hasher.HashPassword(user, password);
    }

    /// <inheritdoc />
    public bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    {
        var result = _hasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success ||
               result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
