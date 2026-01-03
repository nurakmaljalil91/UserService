#nullable enable
using Application.Common.Exceptions;
using Application.Users.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;

namespace Application.UnitTests.Users;

/// <summary>
/// Contains unit tests for <see cref="GetUserByIdQueryHandler"/>.
/// </summary>
public class GetUserByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsUser_WhenFound()
    {
        await using var context = TestDbContextFactory.Create();
        var user = new User
        {
            Username = "user",
            NormalizedUsername = "USER",
            Email = "user@example.com",
            NormalizedEmail = "USER@EXAMPLE.COM",
            IsDeleted = false
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new GetUserByIdQueryHandler(context);

        var result = await handler.Handle(new GetUserByIdQuery { Id = user.Id }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(user.Id, result.Data!.Id);
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenDeleted()
    {
        await using var context = TestDbContextFactory.Create();
        var user = new User
        {
            Username = "user",
            NormalizedUsername = "USER",
            Email = "user@example.com",
            NormalizedEmail = "USER@EXAMPLE.COM",
            IsDeleted = true
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new GetUserByIdQueryHandler(context);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new GetUserByIdQuery { Id = user.Id }, CancellationToken.None));
    }
}
