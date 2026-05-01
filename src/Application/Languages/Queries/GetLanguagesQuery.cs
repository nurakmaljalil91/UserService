#nullable enable
using System;
using Application.Common.Models;
using Application.Languages.Models;
using Domain.Common;
using Mediator;

namespace Application.Languages.Queries;

/// <summary>
/// Represents a request to retrieve languages for a specific user.
/// </summary>
public class GetLanguagesQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<LanguageDto>>>
{
    /// <summary>
    /// Gets or sets the user identifier to filter by.
    /// </summary>
    public Guid? UserId { get; set; }
}
