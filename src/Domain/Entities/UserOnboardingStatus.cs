namespace Domain.Entities;

/// <summary>
/// Tracks the one-time welcome and optional profile setup for an account.
/// </summary>
public enum UserOnboardingStatus
{
    /// <summary>The account is eligible for its first welcome.</summary>
    Pending,

    /// <summary>The welcome has been offered.</summary>
    Prompted,

    /// <summary>The user saved profile details during setup.</summary>
    Completed,

    /// <summary>The user chose to skip profile setup.</summary>
    Skipped
}
