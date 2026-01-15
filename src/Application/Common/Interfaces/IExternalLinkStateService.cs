#nullable enable
using Application.ExternalLinks.Models;
using Domain.ValueObjects;

namespace Application.Common.Interfaces;

/// <summary>
/// Provides creation and validation for external link state values.
/// </summary>
public interface IExternalLinkStateService
{
    /// <summary>
    /// Creates a state value for an external account link request.
    /// </summary>
    /// <param name="userId">The user identifier initiating the link.</param>
    /// <param name="provider">The external provider.</param>
    /// <returns>The generated state value.</returns>
    string CreateState(Guid userId, ExternalProvider provider);

    /// <summary>
    /// Validates the state value and returns parsed state details.
    /// </summary>
    /// <param name="state">The state value to validate.</param>
    /// <returns>The validation result.</returns>
    ExternalLinkStateValidationResult ValidateState(string state);
}
