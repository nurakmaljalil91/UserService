using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator;

/// <summary>
/// Default mediator implementation using DI to resolve handlers.
/// </summary>
public sealed class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve handlers.</param>
    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();

        // Resolve handler: IRequestHandler<TRequest, TResponse>
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var handler = _serviceProvider.GetRequiredService(handlerType);

        // Resolve behaviors: IPipelineBehavior<TRequest, TResponse>[]
        var behaviorInterface = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
        var behaviors = _serviceProvider.GetServices(behaviorInterface).Reverse().ToArray();

        // Terminal delegate that invokes the handler
        RequestHandlerDelegate<TResponse> handlerDelegate = () =>
        {
            var handleMethod = handler.GetType().GetMethod("Handle")!;
            var task = (Task<TResponse>)handleMethod.Invoke(handler, new object[] { request, cancellationToken })!;
            return task;
        };

        // Compose pipeline: last behavior wraps handler, previous wraps next, etc.
        var pipeline = behaviors.Aggregate(handlerDelegate, (next, behavior) =>
        {
            // Use reflection to get the generic arguments for IPipelineBehavior<TRequest, TResponse>
            var behaviorType = behavior.GetType();
            var method = behaviorType.GetMethod("Handle")!;

            // Create a strongly-typed delegate for the behavior
            return () =>
            {
                // Cast request to the expected type for the behavior
                var genericArguments = behaviorType.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>))
                    ?.GetGenericArguments();

                if (genericArguments is not null && genericArguments.Length == 2)
                {
                    var expectedRequestType = genericArguments[0];
                    var castedRequest = Convert.ChangeType(request, expectedRequestType);
                    var task = (Task<TResponse>)method.Invoke(behavior, new object[] { castedRequest, cancellationToken, next })!;
                    return task;
                }
                else
                {
                    // Fallback: invoke as before
                    var task = (Task<TResponse>)method.Invoke(behavior, new object[] { request, cancellationToken, next })!;
                    return task;
                }
            };
        });

        return pipeline();
    }

    /// <inheritdoc/>
    public async Task Publish(INotification notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);

        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
        var handlers = _serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            var handleMethod = handler
                .GetType()
                .GetMethod(nameof(INotificationHandler<INotification>.Handle))!;

            var task = (Task)handleMethod.Invoke(handler, new object[] { notification, cancellationToken })!;
            await task.ConfigureAwait(false);
        }
    }
}
