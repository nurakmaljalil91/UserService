#nullable enable
using Application.Groups.Models;
using Domain.Common;
using Mediator;

namespace Application.Groups.Queries;

/// <summary>
/// Query to retrieve a group by identifier.
/// </summary>
public class GetGroupByIdQuery : IRequest<BaseResponse<GroupDto>>
{
    /// <summary>
    /// Gets or sets the group identifier.
    /// </summary>
    public Guid Id { get; set; }
}
