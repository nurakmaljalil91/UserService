#nullable enable
using System;
using Application.Languages.Models;
using Domain.Common;
using Mediator;

namespace Application.Languages.Queries;

/// <summary>
/// Represents a request to retrieve a single language by its identifier.
/// </summary>
public class GetLanguageByIdQuery : IRequest<BaseResponse<LanguageDto>>
{
    /// <summary>
    /// Gets or sets the language identifier.
    /// </summary>
    public Guid Id { get; set; }
}
