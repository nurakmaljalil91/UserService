#nullable enable
using Domain.Entities;

namespace Application.Common.Interfaces;

/// <summary>
/// Provides password hashing and verification services for user authentication.
/// </summary>
public interface IPasswordHasherService
{
    /// <summary>
    /// Hashes the provided password for the given user.
    /// </summary>
    /// <param name="user">The user for whom the password is being hashed.</param>
    /// <param name="password">The raw password to hash.</param>
    /// <returns>The hashed password.</returns>
    string HashPassword(User user, string password);

    /// <summary>
    /// Verifies that a provided password matches the stored hashed password.
    /// </summary>
    /// <param name="user">The user to validate.</param>
    /// <param name="hashedPassword">The stored hashed password.</param>
    /// <param name="providedPassword">The raw password provided for verification.</param>
    /// <returns><c>true</c> if the password is valid; otherwise, <c>false</c>.</returns>
    bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword);
}
