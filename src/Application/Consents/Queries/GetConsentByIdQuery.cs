#nullable enable
using Application.Consents.Models;
using Domain.Common;
using Mediator;

namespace Application.Consents.Queries;

/// <summary>
/// Query to retrieve a consent by identifier.
/// </summary>
public class GetConsentByIdQuery : IRequest<BaseResponse<ConsentDto>>
{
    /// <summary>
    /// Gets or sets the consent identifier.
    /// </summary>
    public Guid Id { get; set; }
}
