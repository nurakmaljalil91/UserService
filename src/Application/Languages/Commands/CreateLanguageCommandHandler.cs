#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Languages.Models;
using Domain.Common;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Languages.Commands;

/// <summary>
/// Handles creation of a new language record.
/// </summary>
public class CreateLanguageCommandHandler : IRequestHandler<CreateLanguageCommand, BaseResponse<LanguageDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateLanguageCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public CreateLanguageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the creation of a new language record.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created language response.</returns>
    public async Task<BaseResponse<LanguageDto>> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists)
        {
            return BaseResponse<LanguageDto>.Fail("User does not exist.");
        }

        var language = new Language
        {
            UserId = request.UserId,
            Name = request.Name?.Trim(),
            Level = request.Level?.Trim()
        };

        _context.Languages.Add(language);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<LanguageDto>.Ok(new LanguageDto(language), $"Created language with id {language.Id}");
    }
}
