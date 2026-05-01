#nullable enable
using Application.Common.Interfaces;

namespace Application.UnitTests.TestInfrastructure;

/// <summary>
/// Provides deterministic refresh token hashing for unit tests.
/// </summary>
public sealed class TestRefreshTokenHasher : IRefreshTokenHasher
{
    /// <inheritdoc />
    public string Hash(string refreshToken) => $"refresh::{refreshToken}";
}
