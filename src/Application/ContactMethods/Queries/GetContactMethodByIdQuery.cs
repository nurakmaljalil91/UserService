#nullable enable
using Application.ContactMethods.Models;
using Domain.Common;
using Mediator;

namespace Application.ContactMethods.Queries;

/// <summary>
/// Query to retrieve a contact method by identifier.
/// </summary>
public class GetContactMethodByIdQuery : IRequest<BaseResponse<ContactMethodDto>>
{
    /// <summary>
    /// Gets or sets the contact method identifier.
    /// </summary>
    public Guid Id { get; set; }
}
