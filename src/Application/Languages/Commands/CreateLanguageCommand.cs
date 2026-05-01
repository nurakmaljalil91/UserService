#nullable enable
using System;
using Application.Languages.Models;
using Domain.Common;
using Mediator;

namespace Application.Languages.Commands;

/// <summary>
/// Command to create a new language record.
/// </summary>
public class CreateLanguageCommand : IRequest<BaseResponse<LanguageDto>>
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the language name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the proficiency level.
    /// </summary>
    public string? Level { get; set; }
}
