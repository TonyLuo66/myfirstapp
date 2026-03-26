namespace MyFirstApp.Common.Models;

/// <summary>
/// Represents the standard API response envelope.
/// </summary>
/// <typeparam name="TData">The payload type.</typeparam>
public sealed class ApiResponse<TData>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiResponse{TData}"/> class.
    /// </summary>
    /// <param name="rtnCode">The business return code.</param>
    /// <param name="rtnMsg">The business return message.</param>
    /// <param name="data">The response payload.</param>
    public ApiResponse(string rtnCode, string rtnMsg, TData? data)
    {
        rtnCode = string.IsNullOrWhiteSpace(rtnCode) ? string.Empty : rtnCode;
        rtnMsg = string.IsNullOrWhiteSpace(rtnMsg) ? string.Empty : rtnMsg;

        RtnCode = rtnCode;
        RtnMsg = rtnMsg;
        Data = data;
    }

    /// <summary>
    /// Gets the business return code.
    /// </summary>
    public string RtnCode { get; }

    /// <summary>
    /// Gets the business return message.
    /// </summary>
    public string RtnMsg { get; }

    /// <summary>
    /// Gets the response payload.
    /// </summary>
    public TData? Data { get; }

    /// <summary>
    /// Creates a standard success response.
    /// </summary>
    /// <param name="rtnCode">The business return code.</param>
    /// <param name="rtnMsg">The business return message.</param>
    /// <param name="data">The response payload.</param>
    /// <returns>The created response object.</returns>
    public static ApiResponse<TData> Create(string rtnCode, string rtnMsg, TData? data)
    {
        return new ApiResponse<TData>(rtnCode, rtnMsg, data);
    }
}

