using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.TestInfrastructure;

/// <summary>
/// Provides a factory for creating instances of <see cref="TestApplicationDbContext"/> for unit tests.
/// </summary>
public static class TestDbContextFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="TestApplicationDbContext"/> using an in-memory database.
    /// </summary>
    /// <returns>A new <see cref="TestApplicationDbContext"/> instance.</returns>
    public static TestApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestApplicationDbContext(options);
    }
}
