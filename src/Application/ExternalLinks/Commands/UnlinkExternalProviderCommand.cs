#nullable enable
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Constants;
using Domain.ValueObjects;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.ExternalLinks.Commands;

/// <summary>
/// Command to unlink an external provider from the current user.
/// </summary>
public sealed class UnlinkExternalProviderCommand : IRequest<BaseResponse<string>>
{
    /// <summary>
    /// Gets or sets the external provider name.
    /// </summary>
    public string? Provider { get; set; }
}

/// <summary>
/// Handles unlinking an external provider from the current user.
/// </summary>
public sealed class UnlinkExternalProviderCommandHandler : IRequestHandler<UnlinkExternalProviderCommand, BaseResponse<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnlinkExternalProviderCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="user">The current user accessor.</param>
    public UnlinkExternalProviderCommandHandler(
        IApplicationDbContext context,
        IUser user)
    {
        _context = context;
        _user = user;
    }

    /// <inheritdoc />
    public async Task<BaseResponse<string>> Handle(
        UnlinkExternalProviderCommand request,
        CancellationToken cancellationToken)
    {
        var providerValue = string.IsNullOrWhiteSpace(request.Provider)
            ? ExternalProviderNames.Google
            : request.Provider.Trim();

        if (!string.Equals(providerValue, ExternalProviderNames.Google, StringComparison.OrdinalIgnoreCase))
        {
            return BaseResponse<string>.Fail("Unsupported external provider.");
        }

        if (string.IsNullOrWhiteSpace(_user.Username))
        {
            return BaseResponse<string>.Fail("User is not authenticated.");
        }

        var normalizedIdentifier = _user.Username.Trim().ToUpperInvariant();

        var user = await _context.Users.FirstOrDefaultAsync(
            x => x.NormalizedUsername == normalizedIdentifier || x.NormalizedEmail == normalizedIdentifier,
            cancellationToken);

        if (user == null || user.IsDeleted || user.IsLocked)
        {
            return BaseResponse<string>.Fail("User is not available.");
        }

        var provider = ExternalProvider.From(providerValue);

        var identity = await _context.ExternalIdentities
            .FirstOrDefaultAsync(
                x => x.UserId == user.Id && x.Provider == provider,
                cancellationToken);

        var token = await _context.ExternalTokens
            .FirstOrDefaultAsync(
                x => x.UserId == user.Id && x.Provider == provider,
                cancellationToken);

        if (identity == null && token == null)
        {
            return BaseResponse<string>.Fail("External provider is not linked.");
        }

        if (identity != null)
        {
            _context.ExternalIdentities.Remove(identity);
        }

        if (token != null)
        {
            _context.ExternalTokens.Remove(token);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<string>.Ok("External provider unlinked.");
    }
}

/// <summary>
/// Validates the <see cref="UnlinkExternalProviderCommand"/>.
/// </summary>
public sealed class UnlinkExternalProviderCommandValidator : AbstractValidator<UnlinkExternalProviderCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnlinkExternalProviderCommandValidator"/> class.
    /// </summary>
    public UnlinkExternalProviderCommandValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty()
            .WithMessage("Provider is required.");
    }
}
