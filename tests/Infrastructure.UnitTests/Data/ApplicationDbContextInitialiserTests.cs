using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Infrastructure.UnitTests.Data;

/// <summary>
/// Unit tests for <see cref="ApplicationDbContextInitialiser"/>.
/// </summary>
public class ApplicationDbContextInitialiserTests
{
    /// <summary>
    /// Minimal <see cref="IHostEnvironment"/> stub that reports the Development environment,
    /// allowing tests to exercise the development seeding path without introducing a mocking
    /// framework dependency in this test project.
    /// </summary>
    private sealed class DevelopmentEnvironment : IHostEnvironment
    {
        /// <inheritdoc />
        public string EnvironmentName { get; set; } = Environments.Development;

        /// <inheritdoc />
        public string ApplicationName { get; set; } = "Tests";

        /// <inheritdoc />
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();

        /// <inheritdoc />
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    /// <summary>
    /// Ensures that <see cref="ApplicationDbContextInitialiser.SeedEssentialAsync"/> followed by
    /// <see cref="ApplicationDbContextInitialiser.TrySeedAsync"/> adds the expected default data
    /// when the database is empty.
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
        var environment = new DevelopmentEnvironment();
        var initialiser = new ApplicationDbContextInitialiser(logger, context, passwordHasher, environment);

        // Essential seed must run first; TrySeedAsync depends on roles and groups existing.
        await initialiser.SeedEssentialAsync();
        await initialiser.TrySeedAsync();

        Assert.Equal(2, await context.Users.CountAsync());
        Assert.Equal(2, await context.Roles.CountAsync());
        Assert.Equal(3, await context.Groups.CountAsync());
        Assert.NotEmpty(await context.Permissions.ToListAsync());
    }
}
