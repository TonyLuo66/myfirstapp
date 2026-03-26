using System.Runtime.InteropServices;

internal sealed class HeartbeatWorker(
	AppSettings settings,
	AppRuntimeState runtimeState,
	ILogger<HeartbeatWorker> logger,
	IHostApplicationLifetime applicationLifetime) : BackgroundService
{
	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		applicationLifetime.ApplicationStarted.Register(() =>
		{
			logger.LogInformation("HTTP service started. Browse the root page or /health to verify the app.");
		});

		applicationLifetime.ApplicationStopping.Register(() =>
		{
			logger.LogInformation("Shutdown signal received. Stopping HTTP service...");
		});

		applicationLifetime.ApplicationStopped.Register(() =>
		{
			logger.LogInformation("Cleanup completed. Goodbye.");
		});

		if (!settings.IsLongRunning)
		{
			logger.LogInformation("Heartbeat worker is disabled because APP_MODE is set to {RunMode}.", settings.RunMode);
			return Task.CompletedTask;
		}

		logger.LogInformation(
			"Heartbeat worker enabled. Mode: {RunMode}. Interval: {HeartbeatSeconds} second(s).",
			settings.RunMode,
			settings.HeartbeatSeconds);

		return RunHeartbeatLoopAsync(stoppingToken);
	}

	private async Task RunHeartbeatLoopAsync(CancellationToken stoppingToken)
	{
		var tick = 1;

		try
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var heartbeatAtUtc = DateTimeOffset.UtcNow;
				runtimeState.RecordHeartbeat(heartbeatAtUtc);
				logger.LogInformation("Heartbeat {Tick}: {HeartbeatAtUtc:yyyy-MM-dd HH:mm:ss zzz}", tick, heartbeatAtUtc);
				tick++;
				await Task.Delay(TimeSpan.FromSeconds(settings.HeartbeatSeconds), stoppingToken);
			}
		}
		catch (OperationCanceledException)
		{
		}
	}
}
