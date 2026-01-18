using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using WebAPI.Services;

namespace WebAPI.UnitTests.Services;

/// <summary>
/// Unit tests for <see cref="CurrentUser"/>.
/// </summary>
public class CurrentUserTests
{
    [Fact]
    public void Username_ReturnsPreferredUsernameClaim()
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("preferred_username", "alice")
            }))
        };

        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var currentUser = new CurrentUser(accessor);

        Assert.Equal("alice", currentUser.Username);
    }

    [Fact]
    public void GetRoles_ReturnsRoleClaims()
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("role", "Admin"),
                new Claim("role", "Support")
            }))
        };

        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var currentUser = new CurrentUser(accessor);

        var roles = currentUser.GetRoles();

        Assert.Contains("Admin", roles);
        Assert.Contains("Support", roles);
    }

    [Fact]
    public void GetRoles_ReturnsEmptyWhenNoContext()
    {
        var accessor = new HttpContextAccessor();
        var currentUser = new CurrentUser(accessor);

        var roles = currentUser.GetRoles();

        Assert.Empty(roles);
    }

    /// <summary>
    /// Ensures the user identifier is resolved from the subject claim.
    /// </summary>
    [Fact]
    public void UserId_ReturnsSubjectClaim()
    {
        var userId = Guid.NewGuid();
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())
            }))
        };

        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var currentUser = new CurrentUser(accessor);

        Assert.Equal(userId, currentUser.UserId);
    }
}
