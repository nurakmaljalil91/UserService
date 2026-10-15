#nullable enable
using System.Security.Cryptography;

namespace Application.PersonalAccessTokens;

/// <summary>
/// Generates personal access token secrets.
/// </summary>
public static class PersonalAccessTokenSecret
{
    /// <summary>
    /// The fixed prefix of every secret, which lets secret scanners recognize leaked tokens.
    /// </summary>
    public const string Prefix = "cxs_pat_";

    /// <summary>
    /// The number of leading secret characters stored for display.
    /// </summary>
    public const int DisplayPrefixLength = 12;

    /// <summary>
    /// Creates a new secret: <see cref="Prefix"/> followed by 32 random bytes, base64url encoded.
    /// </summary>
    /// <returns>The secret.</returns>
    public static string Create()
    {
        Span<byte> bytes = stackalloc byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Prefix + Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    /// <summary>
    /// Gets the part of the secret stored for display.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>The display prefix.</returns>
    public static string DisplayPrefixOf(string secret) => secret[..Math.Min(DisplayPrefixLength, secret.Length)];
}
