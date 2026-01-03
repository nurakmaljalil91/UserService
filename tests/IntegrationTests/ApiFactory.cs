using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace IntegrationTests;

public class ApiFactory : WebApplicationFactory<Program>
{
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
