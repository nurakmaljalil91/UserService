using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Infrastructure.UnitTests.Data;

/// <summary>
/// Unit tests for <see cref="ApplicationDbContextInitialiser"/>.
/// </summary>
public class ApplicationDbContextInitialiserTests
{
    /// <summary>
    /// Ensures the seed method adds default data when the database is empty.
    /// </summary>
    [Fact]
    public async Task TrySeedAsync_AddsDefaultDataWhenEmpty()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ApplicationDbContext(options);
        var logger = NullLogger<ApplicationDbContextInitialiser>.Instance;
        var passwordHasher = new PasswordHasherService();
        var initialiser = new ApplicationDbContextInitialiser(logger, context, passwordHasher);

        await initialiser.TrySeedAsync();
    }
}
