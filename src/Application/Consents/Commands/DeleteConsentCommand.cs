#nullable enable
using Domain.Common;
using Mediator;

namespace Application.Consents.Commands;

/// <summary>
/// Command to delete a consent by its identifier.
/// </summary>
public class DeleteConsentCommand : IRequest<BaseResponse<string>>
{
    /// <summary>
    /// Gets or sets the consent identifier.
    /// </summary>
    public Guid Id { get; set; }
}
