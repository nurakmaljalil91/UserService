#nullable enable
using Application.Common.Interfaces;

namespace Application.UnitTests.TestInfrastructure;

/// <summary>
/// Provides a test implementation of <see cref="IUser"/>.
/// </summary>
public sealed class TestUser : IUser
{
    /// <summary>
    /// Gets or sets the username value returned by the test user.
    /// </summary>
    public string? Username { get; set; }

    /// <inheritdoc />
    public List<string> GetRoles()
    {
        return new List<string>();
    }
}
