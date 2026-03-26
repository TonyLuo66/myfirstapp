namespace MyFirstApp.Infrastructure.Runtime;

/// <summary>
/// Stores mutable runtime state for the heartbeat worker.
/// </summary>
public sealed class AppRuntimeState
{
    private readonly Lock _syncRoot = new();
    private DateTime? _lastHeartbeatAt;
    private int _heartbeatCount;

    /// <summary>
    /// Records a heartbeat occurrence.
    /// </summary>
    /// <param name="heartbeatAt">The heartbeat timestamp.</param>
    public void RecordHeartbeat(DateTime heartbeatAt)
    {
        lock (_syncRoot)
        {
            _lastHeartbeatAt = heartbeatAt;
            _heartbeatCount++;
        }
    }

    /// <summary>
    /// Creates an immutable snapshot of the current runtime state.
    /// </summary>
    /// <returns>The immutable runtime snapshot.</returns>
    public AppRuntimeSnapshot CreateSnapshot()
    {
        lock (_syncRoot)
        {
            return new AppRuntimeSnapshot(_heartbeatCount, _lastHeartbeatAt);
        }
    }
}
