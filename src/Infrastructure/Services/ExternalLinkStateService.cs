
#nullable enable
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using Application.Common.Interfaces;
using Application.ExternalLinks.Models;
using Domain.ValueObjects;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

/// <summary>
/// Provides HMAC-signed state creation and validation for external account linking.
/// </summary>
public sealed class ExternalLinkStateService : IExternalLinkStateService
{
    private const string SettingsSection = "ExternalLink";
    private const string SigningKeySetting = "StateSigningKey";
    private const string ExpiryMinutesSetting = "StateExpiryMinutes";
    private const int DefaultExpiryMinutes = 15;

    private readonly byte[] _signingKey;
    private readonly int _expiryMinutes;
    private readonly IDateTime _dateTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalLinkStateService"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="dateTime">The date time service.</param>
    public ExternalLinkStateService(IConfiguration configuration, IDateTime dateTime)
    {
        var section = configuration.GetSection(SettingsSection);
        var keyValue = section[SigningKeySetting];

        if (string.IsNullOrWhiteSpace(keyValue))
        {
            throw new InvalidOperationException("External link state signing key is missing.");
        }

        _signingKey = Encoding.UTF8.GetBytes(keyValue);
        _expiryMinutes = section.GetValue<int?>(ExpiryMinutesSetting) ?? DefaultExpiryMinutes;
        _dateTime = dateTime;
    }

    /// <inheritdoc />
    public string CreateState(Guid userId, ExternalProvider provider)
    {
        var issuedAt = new DateTimeOffset(_dateTime.UtcNow).ToUnixTimeSeconds();
        var nonce = Guid.NewGuid().ToString("N");
        var payload = $"{userId}|{provider.Value}|{issuedAt}|{nonce}";
        var signature = ComputeSignature(payload);

        return Base64UrlEncode($"{payload}|{signature}");
    }

    /// <inheritdoc />
    public ExternalLinkStateValidationResult ValidateState(string state)
    {
        if (string.IsNullOrWhiteSpace(state))
        {
            return new ExternalLinkStateValidationResult
            {
                IsValid = false,
                Error = "State is missing."
            };
        }

        string decoded;
        try
        {
            decoded = Base64UrlDecode(state);
        }
        catch (FormatException)
        {
            return new ExternalLinkStateValidationResult
            {
                IsValid = false,
                Error = "State format is invalid."
            };
        }

        var parts = decoded.Split('|');
        if (parts.Length != 5)
        {
            return new ExternalLinkStateValidationResult
            {
                IsValid = false,
                Error = "State format is invalid."
            };
        }

        if (!Guid.TryParse(parts[0], out var userId))
        {
            return new ExternalLinkStateValidationResult
            {
                IsValid = false,
                Error = "State user identifier is invalid."
            };
        }

        var providerValue = parts[1];
        if (string.IsNullOrWhiteSpace(providerValue))
        {
            return new ExternalLinkStateValidationResult
            {
                IsValid = false,
                Error = "State provider is invalid."
            };
        }

        if (!long.TryParse(parts[2], out var issuedAt))
        {
            return new ExternalLinkStateValidationResult
            {
                IsValid = false,
                Error = "State timestamp is invalid."
            };
        }

        var payload = string.Join("|", parts.Take(4));
        var signature = parts[4];

        if (!CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(signature),
                Encoding.UTF8.GetBytes(ComputeSignature(payload))))
        {
            return new ExternalLinkStateValidationResult
            {
                IsValid = false,
                Error = "State signature is invalid."
            };
        }

        var expiresAt = DateTimeOffset.FromUnixTimeSeconds(issuedAt).AddMinutes(_expiryMinutes);
        if (_dateTime.UtcNow > expiresAt.UtcDateTime)
        {
            return new ExternalLinkStateValidationResult
            {
                IsValid = false,
                Error = "State has expired."
            };
        }

        return new ExternalLinkStateValidationResult
        {
            IsValid = true,
            UserId = userId,
            Provider = ExternalProvider.From(providerValue)
        };
    }
    /// <summary>
    /// Computes the HMAC signature for the provided payload.
    /// </summary>
    /// <param name="payload">The payload to sign.</param>
    /// <returns>The Base64-encoded signature.</returns>
    private string ComputeSignature(string payload)
    {
        using var hmac = new HMACSHA256(_signingKey);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Encodes a string to Base64 URL-safe format.
    /// </summary>
    /// <param name="value">The value to encode.</param>
    /// <returns>The Base64 URL-safe encoded string.</returns>
    private static string Base64UrlEncode(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    /// <summary>
    /// Decodes a Base64 URL-safe encoded string.
    /// </summary>
    /// <param name="value">The encoded value.</param>
    /// <returns>The decoded string.</returns>
    private static string Base64UrlDecode(string value)
    {
        var padding = value.Length % 4;
        if (padding != 0)
        {
            value = value.PadRight(value.Length + (4 - padding), '=');
        }

        var base64 = value.Replace('-', '+').Replace('_', '/');
        var bytes = Convert.FromBase64String(base64);
        return Encoding.UTF8.GetString(bytes);
    }
}
