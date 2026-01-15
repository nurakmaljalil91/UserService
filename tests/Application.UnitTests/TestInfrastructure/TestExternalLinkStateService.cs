#nullable enable
using Application.Common.Interfaces;
using Application.ExternalLinks.Models;
using Domain.ValueObjects;

namespace Application.UnitTests.TestInfrastructure;

/// <summary>
/// Provides a test implementation of <see cref="IExternalLinkStateService"/>.
/// </summary>
public sealed class TestExternalLinkStateService : IExternalLinkStateService
{
    /// <summary>
    /// Gets or sets the expected state value.
    /// </summary>
    public string State { get; set; } = "test-state";

    /// <summary>
    /// Gets or sets the user identifier returned when validating state.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the provider returned when validating state.
    /// </summary>
    public ExternalProvider Provider { get; set; } = ExternalProvider.From("google");

    /// <inheritdoc />
    public string CreateState(Guid userId, ExternalProvider provider)
    {
        UserId = userId;
        Provider = provider;
        return State;
    }

    /// <inheritdoc />
    public ExternalLinkStateValidationResult ValidateState(string state)
    {
        if (!string.Equals(state, State, StringComparison.Ordinal))
        {
            return new ExternalLinkStateValidationResult
            {
                IsValid = false,
                Error = "Invalid state."
            };
        }

        return new ExternalLinkStateValidationResult
        {
            IsValid = true,
            UserId = UserId,
            Provider = Provider
        };
    }
}
