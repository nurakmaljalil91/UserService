using Domain.Common;
using System.Net;

namespace Application.Common.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a bad request is encountered in the application.
/// </summary>
public abstract class BadRequestException : Exception
{
    /// <summary>
    /// Gets or sets the error response associated with the bad request.
    /// </summary>
    public BaseResponse<string> ErrorResponse { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class.
    /// </summary>
    protected BadRequestException()
           : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class with the specified details.
    /// </summary>
    /// <param name="details">The details of the bad request.</param>
    protected BadRequestException(string details)
          : base()
    {
        ErrorResponse = BaseResponse<string>.Fail(details, null);
    }
}
