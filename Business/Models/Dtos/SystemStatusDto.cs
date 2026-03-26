namespace MyFirstApp.Business.Models.Dtos;

/// <summary>
/// Represents the service status payload.
/// </summary>
public sealed class SystemStatusDto
{
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
    /// Gets or sets the execution mode source.
    /// </summary>
    public required string RunModeSource { get; init; }

    /// <summary>
    /// Gets or sets the configured heartbeat interval.
    /// </summary>
    public int HeartbeatSeconds { get; init; }

    /// <summary>
    /// Gets or sets the number of observed heartbeats.
    /// </summary>
    public int HeartbeatCount { get; init; }

    /// <summary>
    /// Gets or sets the latest heartbeat timestamp.
    /// </summary>
    public DateTime? LastHeartbeatAt { get; init; }

    /// <summary>
    /// Gets or sets the optional application message.
    /// </summary>
    public string? AppMessage { get; init; }

    /// <summary>
    /// Gets or sets the response timestamp.
    /// </summary>
    public DateTime ReportedAt { get; init; }
}
