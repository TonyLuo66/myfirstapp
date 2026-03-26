using System.Text.Json;
using MyFirstApp.Common.Constants;
using MyFirstApp.Common.Exceptions;
using MyFirstApp.Common.Models;

namespace MyFirstApp.Middleware;

/// <summary>
/// Catches unhandled exceptions and converts them to the standard API response envelope.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next request delegate.</param>
    /// <param name="logger">The logger instance.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Handles the current HTTP request.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task that completes when processing finishes.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException exception)
        {
            _logger.LogWarning(exception, "Application exception. TraceId: {TraceId}, Path: {Path}", context.TraceIdentifier, context.Request.Path);

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = exception.StatusCode;
            context.Response.ContentType = "application/json; charset=utf-8";

            var payload = ApiResponse<ErrorDetailDto>.Create(
                exception.ReturnCode,
                exception.Message,
                new ErrorDetailDto
                {
                    TraceId = context.TraceIdentifier,
                    Path = context.Request.Path.Value ?? string.Empty,
                    OccurredAt = DateTime.UtcNow
                });

            string json = JsonSerializer.Serialize(payload, SerializerOptions);
            await context.Response.WriteAsync(json);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}, Path: {Path}", context.TraceIdentifier, context.Request.Path);

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json; charset=utf-8";

            var payload = ApiResponse<ErrorDetailDto>.Create(
                ReturnCodeConstants.SystemError,
                "系統發生未預期錯誤。",
                new ErrorDetailDto
                {
                    TraceId = context.TraceIdentifier,
                    Path = context.Request.Path.Value ?? string.Empty,
                    OccurredAt = DateTime.UtcNow
                });

            string json = JsonSerializer.Serialize(payload, SerializerOptions);
            await context.Response.WriteAsync(json);
        }
    }
}
