using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exceptions;

/// <summary>
/// Exception thrown when an unsupported colour code is encountered.
/// </summary>
public class UnsupportedColourException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedColourException"/> class with the specified colour code.
    /// </summary>
    /// <param name="code">The unsupported colour code.</param>
    public UnsupportedColourException(string code)
        : base($"Colour \"{code}\" is unsupported.")
    {
    }
}
