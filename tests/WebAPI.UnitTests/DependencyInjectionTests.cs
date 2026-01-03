using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization.Policy;
using WebAPI;
using WebAPI.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.UnitTests;

#nullable enable

/// <summary>
/// Unit tests for WebAPI service registrations.
/// </summary>
public class DependencyInjectionTests
{
    /// <summary>
    /// Verifies that <see cref="WebAPI"/> registers core WebAPI services.
    /// </summary>
    [Fact]
    public void AddWebAPIServices_RegistersCoreServices()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "issuer",
            ["Jwt:Audience"] = "audience",
            ["Jwt:Key"] = "super-secret-key-1234567890"
        });

        services.AddWebAPIServices(configuration);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var scoped = scope.ServiceProvider;

        Assert.NotNull(scoped.GetRequiredService<IHttpContextAccessor>());
        Assert.NotNull(scoped.GetRequiredService<IUser>());
        Assert.NotNull(scoped.GetRequiredService<ExceptionHandlingMiddleware>());
        Assert.NotNull(scoped.GetRequiredService<IAuthorizationMiddlewareResultHandler>());
    }

    private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
        => new ConfigurationBuilder().AddInMemoryCollection(values).Build();
}
#nullable restore
