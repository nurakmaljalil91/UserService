namespace WebAPI.Authorization;

/// <summary>
/// Names of the authorization policies registered by the WebAPI.
/// </summary>
public static class AuthorizationPolicies
{
    /// <summary>
    /// Requires an interactive sign-in. Access tokens exchanged from personal access tokens are rejected,
    /// so they cannot manage tokens, sessions or linked accounts.
    /// </summary>
    public const string InteractiveOnly = "InteractiveOnly";
}
