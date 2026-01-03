using System;
using System.Collections.Generic;
using System.Text;

namespace Mediator;

/// <summary>
/// Represents a request that returns a response of type <typeparamref name="TResponse"/>.
/// </summary>
#pragma warning disable S2326 // Unused type parameters should be removed
public interface IRequest<TResponse>
#pragma warning restore S2326 // Unused type parameters should be removed
{
}
