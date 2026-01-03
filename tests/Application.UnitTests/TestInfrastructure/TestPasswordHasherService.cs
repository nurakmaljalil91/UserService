#nullable enable
using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.UnitTests.TestInfrastructure;

/// <summary>
/// Simple deterministic password hasher for unit tests.
/// </summary>
public sealed class TestPasswordHasherService : IPasswordHasherService
{
    public string HashPassword(User user, string password)
        => $"hashed::{password}";

    public bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        => string.Equals(hashedPassword, HashPassword(user, providedPassword), StringComparison.Ordinal);
}
