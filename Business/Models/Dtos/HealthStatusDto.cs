namespace MyFirstApp.Business.Models.Dtos;

/// <summary>
/// Represents the health check response payload.
/// </summary>
public sealed class HealthStatusDto
{
    /// <summary>
    /// Gets or sets the health status.
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    /// Gets or sets the application name.
    /// </summary>
    public required string AppName { get; init; }

    /// <summary>
    /// Gets or sets the application environment.
    /// </summary>
    public required string AppEnvironment { get; init; }

    /// <summary>
    /// Gets or sets the execution mode.
    /// </summary>
    public required string RunMode { get; init; }

    /// <summary>
    /// Gets or sets the latest heartbeat timestamp.
    /// </summary>
    public DateTime? LastHeartbeatAt { get; init; }

    /// <summary>
    /// Gets or sets the response timestamp.
    /// </summary>
    public DateTime CheckedAt { get; init; }
}
