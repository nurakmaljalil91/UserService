using System;
using System.Collections.Generic;
using System.Text;

namespace Mediator;

/// <summary>
/// Delegate representing the next step in the pipeline (next behavior or the handler).
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();
