using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Application.Common.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Infrastructure.Messaging;
using Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace Infrastructure;

/// <summary>
/// Provides extension methods for registering infrastructure services in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers infrastructure services into the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        var useInMemoryDatabase = configuration.GetValue<bool>("UseInMemoryDatabase");

        // Register health checks
        var healthChecks = services
            .AddHealthChecks()
            .AddCheck("application", () => HealthCheckResult.Healthy());

        if (useInMemoryDatabase)
        {
            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                options
                    .UseInMemoryDatabase("MemoryDb")
                    .AddInterceptors(serviceProvider.GetServices<ISaveChangesInterceptor>());
            });
        }
        else
        {
            // configure DbContext
            var defaultConnection = configuration.GetConnectionString("DefaultConnection");

            Guard.Against.Null(defaultConnection, message: "Connection string 'DefaultConnection' not found.");

            healthChecks.AddNpgSql(
                connectionString: defaultConnection!,
                name: "postgres",
                tags: new[] { "ready"
                });

            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                options
                    .UseNpgsql(defaultConnection,
                        builder =>
                        {
                            builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                            builder.UseNodaTime();
                        })
                    .UseSnakeCaseNamingConvention()
                    .AddInterceptors(serviceProvider.GetServices<ISaveChangesInterceptor>());
            });
        }

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IClockService, ClockService>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IRefreshTokenHasher, RefreshTokenHasher>();
        services.AddTransient<INotificationRequestPublisher, NotificationRequestPublisher>();
        services.AddDataProtection();
        services.AddScoped<IExternalTokenProtector, ExternalTokenProtector>();
        services.AddScoped<IExternalLinkStateService, ExternalLinkStateService>();
        services.AddHttpClient<IGoogleOAuthService, GoogleOAuthService>();

        var messageBrokerSettings = new MessageBrokerSettings();
        configuration.GetSection(MessageBrokerSettings.SectionName).Bind(messageBrokerSettings);
        var useInMemoryMessageBroker = configuration.GetValue<bool>("MessageBroker:UseInMemory");

        if (!useInMemoryMessageBroker)
        {
            var virtualHost = messageBrokerSettings.VirtualHost?.Trim('/');
            var rabbitMqUri = new UriBuilder
            {
                Scheme = "amqp",
                Host = messageBrokerSettings.Host,
                UserName = messageBrokerSettings.Username,
                Password = messageBrokerSettings.Password,
                Path = string.IsNullOrWhiteSpace(virtualHost) ? "/" : $"/{virtualHost}"
            }.Uri;

            healthChecks.AddRabbitMQ(
                serviceProvider =>
                {
                    var factory = new ConnectionFactory
                    {
                        Uri = rabbitMqUri
                    };
                    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
                },
                name: "rabbitmq",
                tags: new[] { "ready" });
        }

        services.AddMassTransit(configurator =>
        {
            if (useInMemoryMessageBroker)
            {
                configurator.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
                return;
            }

            configurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(messageBrokerSettings.Host, messageBrokerSettings.VirtualHost, host =>
                {
                    host.Username(messageBrokerSettings.Username);
                    host.Password(messageBrokerSettings.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
