internal sealed class AppRuntimeState
{
	private readonly Lock _syncRoot = new();
	private DateTimeOffset? _lastHeartbeatAtUtc;
	private int _heartbeatCount;

	public void RecordHeartbeat(DateTimeOffset heartbeatAtUtc)
	{
		lock (_syncRoot)
		{
			_lastHeartbeatAtUtc = heartbeatAtUtc;
			_heartbeatCount++;
		}
	}

	public AppRuntimeSnapshot CreateSnapshot()
	{
		lock (_syncRoot)
		{
			return new AppRuntimeSnapshot(_heartbeatCount, _lastHeartbeatAtUtc);
		}
	}
	}

internal sealed record AppRuntimeSnapshot(int HeartbeatCount, DateTimeOffset? LastHeartbeatAtUtc);
