#nullable enable
using Application.Common.Interfaces;
using Application.ExternalLinks.Models;
using Domain.Common;
using Domain.Constants;
using Domain.ValueObjects;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.ExternalLinks.Commands;

/// <summary>
/// Command to initiate an external account link flow.
/// </summary>
public sealed class StartExternalLinkCommand : IRequest<BaseResponse<ExternalLinkStartResponse>>
{
    /// <summary>
    /// Gets or sets the external provider name.
    /// </summary>
    public string? Provider { get; set; }
}

/// <summary>
/// Handles starting an external account link flow.
/// </summary>
public sealed class StartExternalLinkCommandHandler : IRequestHandler<StartExternalLinkCommand, BaseResponse<ExternalLinkStartResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;
    private readonly IExternalLinkStateService _stateService;
    private readonly IGoogleOAuthService _googleOAuthService;

    /// <summary>
    /// Initializes a new instance of the <see cref="StartExternalLinkCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="user">The current user accessor.</param>
    /// <param name="stateService">The external link state service.</param>
    /// <param name="googleOAuthService">The Google OAuth service.</param>
    public StartExternalLinkCommandHandler(
        IApplicationDbContext context,
        IUser user,
        IExternalLinkStateService stateService,
        IGoogleOAuthService googleOAuthService)
    {
        _context = context;
        _user = user;
        _stateService = stateService;
        _googleOAuthService = googleOAuthService;
    }

    /// <inheritdoc />
    public async Task<BaseResponse<ExternalLinkStartResponse>> Handle(
        StartExternalLinkCommand request,
        CancellationToken cancellationToken)
    {
        var providerValue = string.IsNullOrWhiteSpace(request.Provider)
            ? ExternalProviderNames.Google
            : request.Provider.Trim();

        if (!string.Equals(providerValue, ExternalProviderNames.Google, StringComparison.OrdinalIgnoreCase))
        {
            return BaseResponse<ExternalLinkStartResponse>.Fail("Unsupported external provider.");
        }

        if (string.IsNullOrWhiteSpace(_user.Username))
        {
            return BaseResponse<ExternalLinkStartResponse>.Fail("User is not authenticated.");
        }

        var normalizedIdentifier = _user.Username.Trim().ToUpperInvariant();

        var user = await _context.Users.FirstOrDefaultAsync(
            x => x.NormalizedUsername == normalizedIdentifier || x.NormalizedEmail == normalizedIdentifier,
            cancellationToken);

        if (user == null || user.IsDeleted || user.IsLocked)
        {
            return BaseResponse<ExternalLinkStartResponse>.Fail("User is not available.");
        }

        var provider = ExternalProvider.From(providerValue);
        var state = _stateService.CreateState(user.Id, provider);
        var authorizationUrl = await _googleOAuthService.BuildAuthorizationUrlAsync(state, cancellationToken);

        var response = new ExternalLinkStartResponse
        {
            AuthorizationUrl = authorizationUrl,
            State = state,
            Provider = provider.Value
        };

        return BaseResponse<ExternalLinkStartResponse>.Ok(response, "External link started.");
    }
}

/// <summary>
/// Validates the <see cref="StartExternalLinkCommand"/>.
/// </summary>
public sealed class StartExternalLinkCommandValidator : FluentValidation.AbstractValidator<StartExternalLinkCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartExternalLinkCommandValidator"/> class.
    /// </summary>
    public StartExternalLinkCommandValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty()
            .WithMessage("Provider is required.");
    }
}
