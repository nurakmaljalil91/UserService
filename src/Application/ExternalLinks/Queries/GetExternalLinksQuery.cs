#nullable enable
using Application.Common.Interfaces;
using Application.ExternalLinks.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.ExternalLinks.Queries;

/// <summary>
/// Query to retrieve external links for the current user.
/// </summary>
public sealed class GetExternalLinksQuery : IRequest<BaseResponse<IReadOnlyCollection<ExternalLinkDto>>>
{
    /// <summary>
    /// Gets or sets the external provider name filter.
    /// </summary>
    public string? Provider { get; set; }
}

/// <summary>
/// Handles retrieval of external links for the current user.
/// </summary>
public sealed class GetExternalLinksQueryHandler : IRequestHandler<GetExternalLinksQuery, BaseResponse<IReadOnlyCollection<ExternalLinkDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetExternalLinksQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="user">The current user accessor.</param>
    public GetExternalLinksQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    /// <inheritdoc />
    public async Task<BaseResponse<IReadOnlyCollection<ExternalLinkDto>>> Handle(
        GetExternalLinksQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_user.Username))
        {
            return BaseResponse<IReadOnlyCollection<ExternalLinkDto>>.Fail("User is not authenticated.");
        }

        var normalizedIdentifier = _user.Username.Trim().ToUpperInvariant();

        var user = await _context.Users.FirstOrDefaultAsync(
            x => x.NormalizedUsername == normalizedIdentifier || x.NormalizedEmail == normalizedIdentifier,
            cancellationToken);

        if (user == null || user.IsDeleted || user.IsLocked)
        {
            return BaseResponse<IReadOnlyCollection<ExternalLinkDto>>.Fail("User is not available.");
        }

        var query = _context.ExternalIdentities.AsNoTracking().Where(x => x.UserId == user.Id);

        if (!string.IsNullOrWhiteSpace(request.Provider))
        {
            query = query.Where(x => x.Provider.Value == request.Provider!.Trim().ToLowerInvariant());
        }

        var links = await query
            .OrderBy(x => x.Provider.Value)
            .ToListAsync(cancellationToken);

        var response = links
            .Select(x => new ExternalLinkDto
            {
                Provider = x.Provider.Value,
                SubjectId = x.SubjectId.Value,
                Email = x.Email,
                DisplayName = x.DisplayName,
                LinkedAtUtc = x.LinkedAt.ToDateTimeUtc()
            })
            .ToList()
            .AsReadOnly();

        return BaseResponse<IReadOnlyCollection<ExternalLinkDto>>.Ok(response);
    }
}
