#nullable enable
using System.Collections.Generic;
using Application.Common.Interfaces;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.UnitTests;

/// <summary>
/// Unit tests for infrastructure dependency injection registrations.
/// </summary>
public class DependencyInjectionTests
{
    /// <summary>
    /// Ensures core infrastructure services are registered when using the in-memory database.
    /// </summary>
    [Fact]
    public void AddInfrastructureServices_RegistersServicesForInMemoryDatabase()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped<IUser>(_ => new TestUser("tester"));
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["UseInMemoryDatabase"] = "true"
        });

        services.AddInfrastructureServices(configuration);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var scoped = scope.ServiceProvider;

        var dbContext = scoped.GetRequiredService<ApplicationDbContext>();
        var appDbContext = scoped.GetRequiredService<IApplicationDbContext>();

        Assert.Equal("Microsoft.EntityFrameworkCore.InMemory", dbContext.Database.ProviderName);
        Assert.Same(dbContext, appDbContext);
        Assert.NotNull(scoped.GetRequiredService<IDateTime>());
        Assert.NotNull(scoped.GetRequiredService<IClockService>());
        Assert.NotNull(scoped.GetRequiredService<ApplicationDbContextInitialiser>());

        var interceptors = scoped.GetServices<ISaveChangesInterceptor>();
        Assert.Contains(interceptors, interceptor => interceptor is AuditableEntityInterceptor);
    }

    /// <summary>
    /// Ensures a missing connection string causes registration to fail when not using the in-memory database.
    /// </summary>
    [Fact]
    public void AddInfrastructureServices_ThrowsWhenConnectionStringMissing()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["UseInMemoryDatabase"] = "false"
        });

        Assert.Throws<ArgumentNullException>(() => services.AddInfrastructureServices(configuration));
    }

    private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
        => new ConfigurationBuilder().AddInMemoryCollection(values).Build();

    private sealed class TestUser : IUser
    {
        public TestUser(string username)
        {
            Username = username;
        }

        /// <summary>
        /// Gets the user identifier for the test user.
        /// </summary>
        public Guid? UserId { get; }

        public string? Username { get; }

        public List<string> GetRoles() => new();
    }
}
#nullable restore
