#nullable enable
using Application.UserPreferences.Models;
using Domain.Common;
using Mediator;

namespace Application.UserPreferences.Queries;

/// <summary>
/// Query to retrieve a user preference by identifier.
/// </summary>
public class GetUserPreferenceByIdQuery : IRequest<BaseResponse<UserPreferenceDto>>
{
    /// <summary>
    /// Gets or sets the user preference identifier.
    /// </summary>
    public Guid Id { get; set; }
}
