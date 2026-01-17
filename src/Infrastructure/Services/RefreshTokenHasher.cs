#nullable enable
using System.Security.Cryptography;
using System.Text;
using Application.Common.Interfaces;

namespace Infrastructure.Services;

/// <summary>
/// Hashes refresh tokens using SHA-256 for storage.
/// </summary>
public sealed class RefreshTokenHasher : IRefreshTokenHasher
{
    /// <inheritdoc />
    public string Hash(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return string.Empty;
        }

        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(refreshToken);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
