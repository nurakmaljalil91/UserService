#nullable enable
using Domain.Common;
using Mediator;

namespace Application.ContactMethods.Commands;

/// <summary>
/// Command to delete a contact method by its identifier.
/// </summary>
public class DeleteContactMethodCommand : IRequest<BaseResponse<string>>
{
    /// <summary>
    /// Gets or sets the contact method identifier.
    /// </summary>
    public Guid Id { get; set; }
}
