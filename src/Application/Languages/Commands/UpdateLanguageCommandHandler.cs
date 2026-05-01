#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Languages.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Languages.Commands;

/// <summary>
/// Handles updating a language record.
/// </summary>
public class UpdateLanguageCommandHandler : IRequestHandler<UpdateLanguageCommand, BaseResponse<LanguageDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateLanguageCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UpdateLanguageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the language update request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated language response.</returns>
    public async Task<BaseResponse<LanguageDto>> Handle(UpdateLanguageCommand request, CancellationToken cancellationToken)
    {
        var language = await _context.Languages
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (language == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Language), request.Id.ToString());
        }

        if (request.Name != null)
        {
            language.Name = request.Name.Trim();
        }

        if (request.Level != null)
        {
            language.Level = request.Level.Trim();
        }

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<LanguageDto>.Ok(new LanguageDto(language), "Language updated.");
    }
}
