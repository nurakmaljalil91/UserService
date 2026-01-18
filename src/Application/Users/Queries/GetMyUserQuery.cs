#nullable enable
using Application.Users.Models;
using Domain.Common;
using Mediator;

namespace Application.Users.Queries;

/// <summary>
/// Query to retrieve the current user.
/// </summary>
public class GetMyUserQuery : IRequest<BaseResponse<UserDto>>
{
}
