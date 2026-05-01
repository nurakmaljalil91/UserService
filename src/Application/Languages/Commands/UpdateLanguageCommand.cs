#nullable enable
using System;
using Application.Languages.Models;
using Domain.Common;
using Mediator;

namespace Application.Languages.Commands;

/// <summary>
/// Command to update an existing language record.
/// </summary>
public class UpdateLanguageCommand : IRequest<BaseResponse<LanguageDto>>
{
    /// <summary>
    /// Gets or sets the language identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the language name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the proficiency level.
    /// </summary>
    public string? Level { get; set; }
}
