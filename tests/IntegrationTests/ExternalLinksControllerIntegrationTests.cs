#nullable enable
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces;
using Application.ExternalLinks.Models;
using Domain.Common;
using IntegrationTests.TestInfrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationTests;

/// <summary>
/// Integration tests for the external links controller.
/// </summary>
[Collection("Integration")]
public sealed class ExternalLinksControllerIntegrationTests : ApiTestBase
{
    private const string JwtIssuer = "IntegrationTests";
    private const string JwtAudience = "IntegrationTests";
    private const string JwtKey = "integration-tests-super-secret-key-1234567890";

    private readonly ApiFactory _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalLinksControllerIntegrationTests"/> class.
    /// </summary>
    /// <param name="factory">The API factory used to create the test server and client.</param>
    public ExternalLinksControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Tests that the Google external link flow succeeds.
    /// </summary>
    [Fact]
    public async Task GoogleLinkFlow_Succeeds()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var startResponse = await client.PostAsync("/api/ExternalLinks/google/start", null);
        Assert.Equal(HttpStatusCode.OK, startResponse.StatusCode);
        var startPayload = await ReadResponseAsync<BaseResponse<ExternalLinkStartResponse>>(startResponse);

        var completeResponse = await client.PostAsJsonAsync("/api/ExternalLinks/google/complete", new
        {
            Code = "code",
            State = startPayload.Data!.State
        });

        Assert.Equal(HttpStatusCode.OK, completeResponse.StatusCode);

        var listResponse = await client.GetAsync("/api/ExternalLinks");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var listPayload = await ReadResponseAsync<BaseResponse<IReadOnlyCollection<ExternalLinkDto>>>(listResponse);
        Assert.Contains(listPayload.Data!, link => link.Provider == "google");
    }

    /// <summary>
    /// Tests that a planner service token can fetch a Google Calendar access token.
    /// </summary>
    [Fact]
    public async Task PlannerTokenAccess_ReturnsCalendarToken()
    {
        using var userClient = await CreateAuthenticatedClientAsync();

        var startResponse = await userClient.PostAsync("/api/ExternalLinks/google/start", null);
        var startPayload = await ReadResponseAsync<BaseResponse<ExternalLinkStartResponse>>(startResponse);

        var completeResponse = await userClient.PostAsJsonAsync("/api/ExternalLinks/google/complete", new
        {
            Code = "code",
            State = startPayload.Data!.State
        });
        completeResponse.EnsureSuccessStatusCode();

        // Retrieve the external link state service via the interface to avoid DI issues with concrete type
        var scope = _factory.Services.CreateScope();
        var stateService = scope.ServiceProvider.GetRequiredService<IExternalLinkStateService>();
        var userId = (stateService as TestExternalLinkStateService)?.LastUserId ?? Guid.Empty;

        using var plannerClient = CreateClient();
        plannerClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreatePlannerToken());

        var tokenResponse = await plannerClient.GetAsync($"/api/ExternalLinks/google/calendar-token/{userId}");
        Assert.Equal(HttpStatusCode.OK, tokenResponse.StatusCode);

        var tokenPayload = await ReadResponseAsync<BaseResponse<ExternalAccessTokenDto>>(tokenResponse);
        Assert.True(tokenPayload.Success);
        Assert.False(string.IsNullOrWhiteSpace(tokenPayload.Data!.AccessToken));
    }

    /// <summary>
    /// Creates a JWT token for the planner service.
    /// </summary>
    /// <returns>The serialized JWT token.</returns>
    private static string CreatePlannerToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "planner-service"),
            new Claim("service", "planner")
        };

        var token = new JwtSecurityToken(
            issuer: JwtIssuer,
            audience: JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
