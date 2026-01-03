using System;
using System.Collections.Generic;
using System.Text;

namespace Mediator;

/// <summary>
/// Defines a pipeline behavior that can run before/after the request handler,
/// and optionally short-circuit the pipeline.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Executes the behavior. Call <paramref name="next"/> to invoke the next behavior/handler.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="next">Delegate to invoke the next behavior or the handler.</param>
    /// <returns>Response produced by the pipeline/handler.</returns>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next);
}
