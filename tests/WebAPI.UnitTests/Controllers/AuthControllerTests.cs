using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebAPI.Controllers;

namespace WebAPI.UnitTests.Controllers;

/// <summary>
/// Unit tests for <see cref="AuthController"/>.
/// </summary>
public class AuthControllerTests
{
    [Fact]
    public void Login_ReturnsBadRequestWhenRequestMissingIdentity()
    {
        var controller = new AuthController(BuildConfiguration());

        var result = controller.Login(new LoginRequest(null, " ", null));

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<BaseResponse<AuthTokenResponse>>(badRequest.Value);
        Assert.False(response.Success);
        Assert.Equal("Username or email is required.", response.Message);
    }

    [Fact]
    public void Login_ReturnsServerErrorWhenJwtConfigMissing()
    {
        var controller = new AuthController(BuildConfiguration());

        var result = controller.Login(new LoginRequest("user", null, null));

        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, objectResult.StatusCode);
        var response = Assert.IsType<BaseResponse<AuthTokenResponse>>(objectResult.Value);
        Assert.Equal("JWT configuration is missing.", response.Message);
    }

    [Fact]
    public void Login_ReturnsTokenWithDefaultRole()
    {
        var controller = new AuthController(BuildConfiguration(new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "issuer",
            ["Jwt:Audience"] = "audience",
            ["Jwt:Key"] = "super-secret-key-1234567890-super-secret"
        }));

        var result = controller.Login(new LoginRequest("user", "user@example.com", null));

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<BaseResponse<AuthTokenResponse>>(ok.Value);
        Assert.True(response.Success);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(response.Data!.Token);
        Assert.Equal("issuer", token.Issuer);
        Assert.Contains(token.Claims, claim => claim.Type == ClaimTypes.Role && claim.Value == "Admin");
        Assert.Contains(token.Claims, claim => claim.Type == ClaimTypes.NameIdentifier && claim.Value == "user");
        Assert.Contains(token.Claims, claim => claim.Type == ClaimTypes.Email && claim.Value == "user@example.com");
    }

    [Fact]
    public void Login_UsesExplicitRolesWhenProvided()
    {
        var controller = new AuthController(BuildConfiguration(new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "issuer",
            ["Jwt:Audience"] = "audience",
            ["Jwt:Key"] = "super-secret-key-1234567890-super-secret"
        }));

        var result = controller.Login(new LoginRequest("user", null, new[] { "Admin", "Support" }));

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<BaseResponse<AuthTokenResponse>>(ok.Value);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(response.Data!.Token);
        Assert.Contains(token.Claims, claim => claim.Type == ClaimTypes.Role && claim.Value == "Admin");
        Assert.Contains(token.Claims, claim => claim.Type == ClaimTypes.Role && claim.Value == "Support");
    }

    private static IConfiguration BuildConfiguration(Dictionary<string, string?>? values = null)
        => new ConfigurationBuilder()
            .AddInMemoryCollection(values ?? new Dictionary<string, string?>())
            .Build();
}
