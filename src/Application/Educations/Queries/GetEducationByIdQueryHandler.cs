#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Educations.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Educations.Queries;

/// <summary>
/// Handles retrieval of an education record by identifier.
/// </summary>
public class GetEducationByIdQueryHandler : IRequestHandler<GetEducationByIdQuery, BaseResponse<EducationDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEducationByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetEducationByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the education record lookup request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The education record response.</returns>
    public async Task<BaseResponse<EducationDto>> Handle(GetEducationByIdQuery request, CancellationToken cancellationToken)
    {
        var education = await _context.Educations
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (education == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Education), request.Id.ToString());
        }

        return BaseResponse<EducationDto>.Ok(new EducationDto(education), "Education record retrieved.");
    }
}
