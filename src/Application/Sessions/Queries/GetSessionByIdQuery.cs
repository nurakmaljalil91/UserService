#nullable enable
using Application.Sessions.Models;
using Domain.Common;
using Mediator;

namespace Application.Sessions.Queries;

/// <summary>
/// Query to retrieve a session by identifier.
/// </summary>
public class GetSessionByIdQuery : IRequest<BaseResponse<SessionDto>>
{
    /// <summary>
    /// Gets or sets the session identifier.
    /// </summary>
    public Guid Id { get; set; }
}
