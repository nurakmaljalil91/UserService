#nullable enable
namespace Application.Common.Interfaces;

/// <summary>
/// Protects and unprotects sensitive external token data for storage and retrieval.
/// </summary>
public interface IExternalTokenProtector
{
    /// <summary>
    /// Protects the provided token value for persistence.
    /// </summary>
    /// <param name="value">The raw token value.</param>
    /// <returns>The protected token value.</returns>
    string Protect(string value);

    /// <summary>
    /// Unprotects the provided token value for use.
    /// </summary>
    /// <param name="value">The protected token value.</param>
    /// <returns>The raw token value.</returns>
    string Unprotect(string value);
}
