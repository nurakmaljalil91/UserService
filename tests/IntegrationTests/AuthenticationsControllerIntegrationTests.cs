using System.Net;
using System.Net.Http.Json;
using Application.Authentications.Models;
using Domain.Common;
using WebAPI.Controllers;

namespace IntegrationTests;

/// <summary>
/// Integration tests for the <see cref="AuthenticationsController"/> endpoints.
/// </summary>
[Collection("Integration")]
public class AuthenticationsControllerIntegrationTests : ApiTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationsControllerIntegrationTests"/> class.
    /// </summary>
    /// <param name="factory">The API factory used to create the test server and client.</param>
    public AuthenticationsControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    /// <summary>
    /// Tests that the login endpoint returns a valid authentication token for a newly registered user.
    /// </summary>
    [Fact]
    public async Task Login_ReturnsToken()
    {
        using var client = CreateClient();

        var unique = Guid.NewGuid().ToString("N");
        var username = $"user-{unique}";
        var email = $"user-{unique}@example.com";

        var registerResponse = await client.PostAsJsonAsync("/api/Authentications/register", new
        {
            Username = username,
            Email = email,
            Password = "pass123!"
        });

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var response = await client.PostAsJsonAsync("/api/Authentications/login", new
        {
            Username = username,
            Email = email,
            Password = "pass123!"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await ReadResponseAsync<BaseResponse<LoginResponse>>(response);
        Assert.True(payload.Success);
        Assert.NotNull(payload.Data);
        Assert.False(string.IsNullOrWhiteSpace(payload.Data!.Token));
        Assert.True(payload.Data!.ExpiresAt > DateTime.UtcNow.AddMinutes(-1));
    }
}
