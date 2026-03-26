namespace MyFirstApp.Common.Exceptions;

/// <summary>
/// Represents an application-level exception with a business return code.
/// </summary>
public sealed class AppException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppException"/> class.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="returnCode">The business return code.</param>
    /// <param name="message">The business error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public AppException(int statusCode, string returnCode, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ReturnCode = returnCode;
    }

    /// <summary>
    /// Gets the HTTP status code.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets the business return code.
    /// </summary>
    public string ReturnCode { get; }
}
