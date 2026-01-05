#nullable enable
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace IntegrationTests;

/// <summary>
/// Provides a custom <see cref="WebApplicationFactory{TEntryPoint}"/> for integration testing,
/// configuring the web host to use in-memory settings and a development environment.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Configures the web host for integration testing by setting the environment to Development
    /// and adding in-memory configuration settings for the test context.
    /// </summary>
    /// <param name="builder">The <see cref="IWebHostBuilder"/> to configure.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["UseInMemoryDatabase"] = "true",
                ["Jwt:Issuer"] = "IntegrationTests",
                ["Jwt:Audience"] = "IntegrationTests",
                ["Jwt:Key"] = "integration-tests-super-secret-key-1234567890",
                ["Jwt:ExpiryMinutes"] = "60"
            };

            config.AddInMemoryCollection(settings);
        });
    }
}
