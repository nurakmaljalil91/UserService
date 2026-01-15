#nullable enable
using Application.Common.Interfaces;
using Application.ExternalLinks.Models;
using Domain.ValueObjects;

namespace IntegrationTests.TestInfrastructure;

/// <summary>
/// Provides a test implementation of <see cref="IExternalLinkStateService"/> for integration tests.
/// </summary>
public sealed class TestExternalLinkStateService : IExternalLinkStateService
{
    /// <summary>
    /// Gets the last user identifier captured during state creation.
    /// </summary>
    public Guid LastUserId { get; private set; }

    /// <summary>
    /// Gets the last provider captured during state creation.
    /// </summary>
    public ExternalProvider LastProvider { get; private set; } = ExternalProvider.From("google");

    /// <summary>
    /// Gets the fixed state value returned by this service.
    /// </summary>
    public string State { get; } = "integration-state";

    /// <inheritdoc />
    public string CreateState(Guid userId, ExternalProvider provider)
    {
        LastUserId = userId;
        LastProvider = provider;
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
            UserId = LastUserId,
            Provider = LastProvider
        };
    }
}
