#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Languages.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Languages.Queries;

/// <summary>
/// Handles retrieval of a language by its identifier.
/// </summary>
public class GetLanguageByIdQueryHandler : IRequestHandler<GetLanguageByIdQuery, BaseResponse<LanguageDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetLanguageByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetLanguageByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the get language by id request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The language response.</returns>
    public async Task<BaseResponse<LanguageDto>> Handle(GetLanguageByIdQuery request, CancellationToken cancellationToken)
    {
        var language = await _context.Languages
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (language == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Language), request.Id.ToString());
        }

        return BaseResponse<LanguageDto>.Ok(new LanguageDto(language), "Language retrieved.");
    }
}
