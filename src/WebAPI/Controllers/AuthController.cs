using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebAPI.Controllers;

/// <summary>
/// Issues JWT tokens for clients. Replace with a real authentication flow later.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Returns a JWT token for the supplied identity. For demo/dev use only.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult<BaseResponse<AuthTokenResponse>> Login([FromBody] LoginRequest request)
    {
        if (request == null || (string.IsNullOrWhiteSpace(request.Username) && string.IsNullOrWhiteSpace(request.Email)))
        {
            return BadRequest(BaseResponse<AuthTokenResponse>.Fail("Username or email is required."));
        }

        var jwtSection = _configuration.GetSection("Jwt");
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var key = jwtSection["Key"];

        if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience) || string.IsNullOrWhiteSpace(key))
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                BaseResponse<AuthTokenResponse>.Fail("JWT configuration is missing."));
        }

        var expiryMinutes = 60;
        if (int.TryParse(jwtSection["ExpiryMinutes"], out var configuredMinutes) && configuredMinutes > 0)
        {
            expiryMinutes = configuredMinutes;
        }

        var identity = string.IsNullOrWhiteSpace(request.Username) ? request.Email! : request.Username!;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, identity)
        };

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, request.Email));
        }

        if (request.Roles is { Length: > 0 })
        {
            foreach (var role in request.Roles)
            {
                if (!string.IsNullOrWhiteSpace(role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(BaseResponse<AuthTokenResponse>.Ok(new AuthTokenResponse(tokenValue, expires), "Token issued."));
    }
}

public sealed record LoginRequest(string? Username, string? Email, string[]? Roles);

public sealed record AuthTokenResponse(string Token, DateTime ExpiresAtUtc);
