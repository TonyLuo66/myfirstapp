namespace MyFirstApp.Infrastructure.Runtime;

/// <summary>
/// Represents the current heartbeat runtime snapshot.
/// </summary>
public sealed class AppRuntimeSnapshot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppRuntimeSnapshot"/> class.
    /// </summary>
    /// <param name="heartbeatCount">The number of heartbeats observed.</param>
    /// <param name="lastHeartbeatAt">The latest heartbeat timestamp.</param>
    public AppRuntimeSnapshot(int heartbeatCount, DateTime? lastHeartbeatAt)
    {
        HeartbeatCount = heartbeatCount;
        LastHeartbeatAt = lastHeartbeatAt;
    }

    /// <summary>
    /// Gets the number of heartbeats observed.
    /// </summary>
    public int HeartbeatCount { get; }

    /// <summary>
    /// Gets the timestamp of the latest heartbeat.
    /// </summary>
    public DateTime? LastHeartbeatAt { get; }
}
