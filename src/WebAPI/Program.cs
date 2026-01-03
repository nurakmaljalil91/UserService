using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;
using WebAPI;
using WebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Read Serilog configuration from appsettings
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    var dateTimeUtcNow = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);

    Log.Information("Starting Web Application");

    Log.Information("UTC Time: {DateTimeUtcNow}", dateTimeUtcNow);

    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddWebAPIServices(builder.Configuration);

    builder.Services.AddControllers();

    var app = builder.Build();

    app.UseMiddleware<CorrelationIdMiddleware>();

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        // Add OpenAPI 3.0 document serving middleware
        app.UseOpenApi();

        // Add web UIs to interact with the document
        app.UseSwaggerUi();

        // Add ReDoc UI to interact with the document
        app.UseReDoc(options => { options.Path = "/redoc"; });

        // Initialize and seed database
        using (var scope = app.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
            await initializer.InitialiseAsync();
            await initializer.SeedAsync();
        }
    }

    app.UseHttpsRedirection();

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

    app.UseCors("CorsPolicy");

    app.UseAuthentication();

    app.UseAuthorization();
    // Liveness: just "is the process up?"
    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = _ => false // don't run any checks, always "Healthy" if app is running
    }).AllowAnonymous();

    // Readiness: run all registered checks (DB, etc.)
    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description
                }),
                duration = report.TotalDuration.TotalMilliseconds
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            await context.Response.WriteAsync(json);
        }
    }).AllowAnonymous();

    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    // Ensure fatal startup errors are captured
    Log.Fatal(ex, "Web Application start-up failed");
}
finally
{
    // Flush and close loggers
    await Log.CloseAndFlushAsync();
}
