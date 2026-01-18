#nullable enable
using Application.Common.Interfaces;
using Application.Groups.Models;
using Domain.Common;
using Domain.Entities;
using Mediator;

namespace Application.Groups.Commands;

/// <summary>
/// Handles creation of a new group.
/// </summary>
public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, BaseResponse<GroupDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateGroupCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public CreateGroupCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the creation of a new group.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created group response.</returns>
    public async Task<BaseResponse<GroupDto>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name!.Trim();
        var normalizedName = name.ToUpperInvariant();

        var group = new Group
        {
            Name = name,
            NormalizedName = normalizedName,
            Description = request.Description?.Trim()
        };

        _context.Groups.Add(group);
        await _context.SaveChangesAsync(cancellationToken);

        return BaseResponse<GroupDto>.Ok(new GroupDto(group), $"Created group with id {group.Id}");
    }
}
