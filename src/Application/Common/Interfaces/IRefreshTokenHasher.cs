#nullable enable
namespace Application.Common.Interfaces;

/// <summary>
/// Provides hashing for refresh tokens so they can be stored securely.
/// </summary>
public interface IRefreshTokenHasher
{
    /// <summary>
    /// Hashes a refresh token into a deterministic representation.
    /// </summary>
    /// <param name="refreshToken">The raw refresh token.</param>
    /// <returns>The hashed refresh token.</returns>
    string Hash(string refreshToken);
}
