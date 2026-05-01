#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Languages.Commands;

/// <summary>
/// Handles deletion of a language record.
/// </summary>
public class DeleteLanguageCommandHandler : IRequestHandler<DeleteLanguageCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteLanguageCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public DeleteLanguageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the language deletion request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A success or failure response.</returns>
    public async Task<BaseResponse<string>> Handle(DeleteLanguageCommand request, CancellationToken cancellationToken)
    {
        var language = await _context.Languages
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (language == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Language), request.Id.ToString());
        }

        _context.Languages.Remove(language);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<string>.Ok(request.Id.ToString(), "Language deleted.");
    }
}
