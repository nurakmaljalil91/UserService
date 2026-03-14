#nullable enable
using System.Net;
using Domain.Common;

namespace IntegrationTests;

/// <summary>
/// Integration tests for the <see cref="WebAPI.Controllers.StatusController"/>.
/// </summary>
[Collection("Integration")]
public class StatusControllerIntegrationTests : ApiTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StatusControllerIntegrationTests"/> class.
    /// </summary>
    /// <param name="factory">The API factory.</param>
    public StatusControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    /// <summary>
    /// Verifies that the status endpoint returns the configured build version.
    /// </summary>
    [Fact]
    public async Task GetStatus_ReturnsApiStatus()
    {
        using var client = CreateClient();

        var response = await client.GetAsync("/api/Status");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await ReadResponseAsync<ApiStatus>(response);
        Assert.Equal("Online", payload.Status);
        Assert.Equal("integration-test-build", payload.BuildVersion);
    }
}
