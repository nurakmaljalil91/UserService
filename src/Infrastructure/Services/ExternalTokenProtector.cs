#nullable enable
using Application.Common.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace Infrastructure.Services;

/// <summary>
/// Protects and unprotects external OAuth token values using ASP.NET Core data protection.
/// </summary>
public sealed class ExternalTokenProtector : IExternalTokenProtector
{
    private const string ProtectorPurpose = "ExternalTokenProtector.v1";
    private readonly IDataProtector _protector;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalTokenProtector"/> class.
    /// </summary>
    /// <param name="provider">The data protection provider.</param>
    public ExternalTokenProtector(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector(ProtectorPurpose);
    }

    /// <inheritdoc />
    public string Protect(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Token value is required.", nameof(value));
        }

        return _protector.Protect(value);
    }

    /// <inheritdoc />
    public string Unprotect(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Token value is required.", nameof(value));
        }

        return _protector.Unprotect(value);
    }
}
