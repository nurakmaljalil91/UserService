#nullable enable
using Application.ContactMethods.Models;
using Domain.Common;
using Mediator;

namespace Application.ContactMethods.Commands;

/// <summary>
/// Command to create a new contact method.
/// </summary>
public class CreateContactMethodCommand : IRequest<BaseResponse<ContactMethodDto>>
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the contact method type.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the contact value (email or phone number).
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets whether the contact method is verified.
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// Gets or sets whether the contact method is the primary one.
    /// </summary>
    public bool IsPrimary { get; set; }
}
