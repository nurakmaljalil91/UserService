using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Authentications.Models;
using Domain.Common;

namespace IntegrationTests;

/// <summary>
/// Integration tests for the time zone preference and the <c>zoneinfo</c> access-token claim.
/// </summary>
[Collection("Integration")]
public class TimeZonePreferenceIntegrationTests : ApiTestBase
{
    private const string Password = "Integration123!";

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeZonePreferenceIntegrationTests"/> class.
    /// </summary>
    /// <param name="factory">The API factory used to create test clients.</param>
    public TimeZonePreferenceIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    /// <summary>
    /// Ensures an invalid time zone value is rejected with 400 Bad Request.
    /// </summary>
    [Fact]
    public async Task CreateTimeZonePreference_WithOffset_ReturnsBadRequest()
    {
        var (client, userId) = await CreateAuthenticatedClientWithUserAsync();
        using (client)
        {
            var response = await client.PostAsJsonAsync("/api/UserPreferences", new
            {
                UserId = userId,
                Key = "timezone",
                Value = "UTC+7"
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    /// <summary>
    /// Ensures that after saving a time zone, the refreshed access token carries it as the <c>zoneinfo</c> claim.
    /// </summary>
    [Fact]
    public async Task RefreshToken_AfterSavingTimeZone_IncludesZoneInfoClaim()
    {
        using var client = CreateClient();
        var login = await RegisterAndLoginAsync(client);

        var initial = new JwtSecurityTokenHandler().ReadJwtToken(login.Token);
        Assert.DoesNotContain(initial.Claims, c => c.Type == "zoneinfo");
        var userId = Guid.Parse(initial.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Sub).Value);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.Token);
        var createResponse = await client.PostAsJsonAsync("/api/UserPreferences", new
        {
            UserId = userId,
            Key = "timezone",
            Value = "Asia/Kuala_Lumpur"
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var refreshResponse = await client.PostAsJsonAsync("/api/Authentications/refresh", new
        {
            login.RefreshToken
        });
        refreshResponse.EnsureSuccessStatusCode();
        var refreshed = await ReadResponseAsync<BaseResponse<LoginResponse>>(refreshResponse);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(refreshed.Data!.Token);
        Assert.Equal("Asia/Kuala_Lumpur", token.Claims.Single(c => c.Type == "zoneinfo").Value);
    }

    private static async Task<LoginResponse> RegisterAndLoginAsync(HttpClient client)
    {
        var unique = Guid.NewGuid().ToString("N");
        var username = $"tz-user-{unique}";

        var registerResponse = await client.PostAsJsonAsync("/api/Authentications/register", new
        {
            Username = username,
            Email = $"tz-{unique}@example.com",
            Password
        });
        registerResponse.EnsureSuccessStatusCode();

        var loginResponse = await client.PostAsJsonAsync("/api/Authentications/login", new
        {
            Username = username,
            Password
        });
        loginResponse.EnsureSuccessStatusCode();

        var payload = await ReadResponseAsync<BaseResponse<LoginResponse>>(loginResponse);
        Assert.True(payload.Success);
        return payload.Data!;
    }
}
