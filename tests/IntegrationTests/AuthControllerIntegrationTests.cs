using System.Net;
using System.Net.Http.Json;
using Domain.Common;

namespace IntegrationTests;

[Collection("Integration")]
public class AuthControllerIntegrationTests : ApiTestBase
{
    public AuthControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Login_ReturnsToken()
    {
        using var client = CreateClient();

        var response = await client.PostAsJsonAsync("/api/Auth/login", new
        {
            Username = "user",
            Email = "user@example.com"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await ReadResponseAsync<BaseResponse<AuthTokenResponse>>(response);
        Assert.True(payload.Success);
        Assert.NotNull(payload.Data);
        Assert.False(string.IsNullOrWhiteSpace(payload.Data!.Token));
        Assert.True(payload.Data!.ExpiresAtUtc > DateTime.UtcNow.AddMinutes(-1));
    }
}
