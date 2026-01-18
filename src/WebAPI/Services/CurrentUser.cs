#nullable enable
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Common.Interfaces;

namespace WebAPI.Services;

/// <summary>
/// Provides information about the current authenticated user based on the HTTP context.
/// </summary>
public class CurrentUser : IUser
{
    private const string RoleClaimType = "role";
    private const string PreferredUsernameClaimType = "preferred_username";
    private const string SubjectClaimType = JwtRegisteredClaimNames.Sub;

    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUser"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc/>
    public Guid? UserId
    {
        get
        {
            var subject = _httpContextAccessor.HttpContext?.User?.FindFirstValue(SubjectClaimType);
            return Guid.TryParse(subject, out var userId) ? userId : null;
        }
    }

    /// <inheritdoc/>
    public string? Username => _httpContextAccessor.HttpContext?.User?.FindFirstValue(PreferredUsernameClaimType);

    /// <summary>
    /// Gets the roles associated with the current authenticated user.
    /// </summary>  
    /// <remarks>
    /// This method returns a new list containing the user's roles.
    /// </remarks>
    public List<string> GetRoles()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user is not null
            ? user.FindAll(RoleClaimType).Select(x => x.Value).ToList()
            : new List<string>();
    }
}
#nullable restore
