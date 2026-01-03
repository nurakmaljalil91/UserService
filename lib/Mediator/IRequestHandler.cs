using System;
using System.Collections.Generic;
using System.Text;

namespace Mediator;

/// <summary>
/// Handles a request of type <typeparamref name="TRequest"/> and returns <typeparamref name="TResponse"/>.
/// </summary>
public interface IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the specified request and returns a response.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response.</returns>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
