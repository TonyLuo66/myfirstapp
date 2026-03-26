namespace MyFirstApp.Middleware;

/// <summary>
/// Logs request and response summary information.
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestLoggingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next request delegate.</param>
    /// <param name="logger">The logger instance.</param>
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
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
        long startedAt = Environment.TickCount64;

        await _next(context);

        long elapsedMilliseconds = Environment.TickCount64 - startedAt;
        _logger.LogInformation(
            "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            elapsedMilliseconds);
    }
}
