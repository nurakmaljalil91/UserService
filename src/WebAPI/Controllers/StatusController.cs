using Ardalis.GuardClauses;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// Represents the API controller responsible for providing status information about the application, including its online status and build version.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class StatusController : ControllerBase
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatusController"/> class with the specified configuration.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    public StatusController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the current status of the API, including whether it is online and its build version.
    /// </summary>
    /// <returns>The API status payload.</returns>
    [HttpGet]
    [AllowAnonymous]
    public Task<ActionResult> GetStatus()
    {
        var buildVersion = _configuration.GetValue<string>("BuildVersion");

        Guard.Against.NullOrEmpty(buildVersion, message: "Build version is not found");

        return Task.FromResult<ActionResult>(Ok(new ApiStatus
        {
            Status = "Online",
            BuildVersion = buildVersion,
        }));
    }
}
