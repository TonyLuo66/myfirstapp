namespace MyFirstApp.Common.Models;

/// <summary>
/// Represents the standard error payload.
/// </summary>
public sealed class ErrorDetailDto
{
    /// <summary>
    /// Gets or sets the request trace identifier.
    /// </summary>
    public required string TraceId { get; init; }

    /// <summary>
    /// Gets or sets the request path.
    /// </summary>
    public required string Path { get; init; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the error occurred.
    /// </summary>
    public DateTime OccurredAt { get; init; }
}
