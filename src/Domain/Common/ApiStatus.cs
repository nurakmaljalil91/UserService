#nullable enable
namespace Domain.Common;

/// <summary>
/// Represents the status information for the API, including its current state and build version.
/// </summary>
public class ApiStatus
{
    /// <summary>
    /// Gets or sets the current status of the API.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the build version of the API.
    /// </summary>
    public string? BuildVersion { get; set; }
}
