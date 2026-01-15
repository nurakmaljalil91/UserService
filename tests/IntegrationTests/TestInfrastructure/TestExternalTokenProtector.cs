#nullable enable
using Application.Common.Interfaces;

namespace IntegrationTests.TestInfrastructure;

/// <summary>
/// Provides a test implementation of <see cref="IExternalTokenProtector"/> for integration tests.
/// </summary>
public sealed class TestExternalTokenProtector : IExternalTokenProtector
{
    private const string Prefix = "protected::";

    /// <inheritdoc />
    public string Protect(string value)
    {
        return $"{Prefix}{value}";
    }

    /// <inheritdoc />
    public string Unprotect(string value)
    {
        return value.StartsWith(Prefix, StringComparison.Ordinal)
            ? value.Substring(Prefix.Length)
            : value;
    }
}
