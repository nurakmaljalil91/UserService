#nullable enable
using System;
using Domain.Common;
using Mediator;

namespace Application.Languages.Commands;

/// <summary>
/// Command to delete a language record.
/// </summary>
public class DeleteLanguageCommand : IRequest<BaseResponse<string>>
{
    /// <summary>
    /// Gets or sets the language identifier.
    /// </summary>
    public Guid Id { get; set; }
}
