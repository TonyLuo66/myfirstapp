using Microsoft.AspNetCore.Mvc;
using MyFirstApp.Common.Constants;
using MyFirstApp.Common.Models;

namespace MyFirstApp.Controllers;

/// <summary>
/// Provides shared API response helpers.
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Creates a successful API response payload.
    /// </summary>
    /// <typeparam name="TData">The payload type.</typeparam>
    /// <param name="data">The response payload.</param>
    /// <param name="message">The success message.</param>
    /// <returns>The wrapped action result.</returns>
    protected ActionResult<ApiResponse<TData>> Success<TData>(TData data, string message = "success")
    {
        return Ok(new ApiResponse<TData>(ReturnCodeConstants.Success, message, data));
    }

    /// <summary>
    /// Creates a validation error API response payload.
    /// </summary>
    /// <typeparam name="TData">The payload type.</typeparam>
    /// <param name="message">The validation message.</param>
    /// <returns>The wrapped action result.</returns>
    protected ActionResult<ApiResponse<TData>> ValidationError<TData>(string message)
    {
        return BadRequest(new ApiResponse<TData>(ReturnCodeConstants.ValidationError, message, default));
    }

    /// <summary>
    /// Creates a custom failure API response payload.
    /// </summary>
    /// <typeparam name="TData">The payload type.</typeparam>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="rtnCode">The business return code.</param>
    /// <param name="message">The failure message.</param>
    /// <returns>The wrapped action result.</returns>
    protected ActionResult<ApiResponse<TData>> Failure<TData>(int statusCode, string rtnCode, string message)
    {
        return StatusCode(statusCode, new ApiResponse<TData>(rtnCode, message, default));
    }
}
