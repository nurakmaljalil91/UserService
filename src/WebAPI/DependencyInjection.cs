using Application.Common.Interfaces;
using NSwag;
using NSwag.Generation.Processors.Security;
using WebAPI.Middlewares;
using WebAPI.Services;

using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Authorization;

namespace WebAPI;

/// <summary>
/// Provides extension methods for registering WebAPI services in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers WebAPI services into the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register HttpContextAccessor
        services.AddHttpContextAccessor();

        services.AddScoped<IUser, CurrentUser>();

        // Allow all origin since it will be web service
        services.AddCors(options =>
        {
            options.AddPolicy(
                name: "CorsPolicy",
                policy =>
                {
                    policy
                        .SetIsOriginAllowed(origin => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials(); // Required for SignalR
                });
        });

        // Register OpenAPI document generation
        services.AddOpenApiDocument(options =>
        {
            options.PostProcess = document =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "Web System",
                    Version = "v1",
                    Description = "System Web API",
                    TermsOfService = "https://example.com/terms",
                    Contact = new OpenApiContact
                    {
                        Name = "Nur Akmal Bin Jalil",
                        Email = "nurakmaljalil91@gmail.com",
                        Url = "https://nurakmaljalil.com/"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Example License",
                        Url = "https://example.com/license"
                    }
                };
            };

            // Add JWT token authorization
            options.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Type into the text box: Bearer {your JWT token}."
            });

            options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtSection = configuration.GetSection("Jwt");
                var issuer = jwtSection["Issuer"];
                var audience = jwtSection["Audience"];
                var key = jwtSection["Key"];

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key ?? string.Empty)),
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationMiddlewareResultHandler>();

        services.AddTransient<ExceptionHandlingMiddleware>();

        return services;
    }
}
