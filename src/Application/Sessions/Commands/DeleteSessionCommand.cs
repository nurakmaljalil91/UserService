#nullable enable
using Domain.Common;
using Mediator;

namespace Application.Sessions.Commands;

/// <summary>
/// Command to delete a session by its identifier.
/// </summary>
public class DeleteSessionCommand : IRequest<BaseResponse<string>>
{
    /// <summary>
    /// Gets or sets the session identifier.
    /// </summary>
    public Guid Id { get; set; }
}
