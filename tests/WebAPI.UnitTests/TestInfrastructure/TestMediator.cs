using Mediator;

namespace WebAPI.UnitTests.TestInfrastructure;

public sealed class TestMediator : IMediator
{
    private readonly Func<object, Task<object>> _handler;

    public TestMediator(Func<object, Task<object>> handler)
    {
        _handler = handler;
    }

    public object? LastRequest { get; private set; }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        LastRequest = request;
        var result = await _handler(request);
        return (TResponse)result;
    }

    public Task Publish(INotification notification, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
