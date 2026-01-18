#nullable enable
using Application.UserSessions.Models;
using Domain.Common;
using Mediator;

namespace Application.UserSessions.Queries;

/// <summary>
/// Query to retrieve the current user's session details.
/// </summary>
public class GetUserSessionQuery : IRequest<BaseResponse<UserSessionDto>>
{
}
