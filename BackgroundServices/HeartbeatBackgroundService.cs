using MyFirstApp.Configuration;
using MyFirstApp.Infrastructure.Runtime;

namespace MyFirstApp.BackgroundServices;

/// <summary>
/// Executes the background heartbeat loop for the running service.
/// </summary>
public sealed class HeartbeatBackgroundService : BackgroundService
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger<HeartbeatBackgroundService> _logger;
    private readonly AppRuntimeState _runtimeState;
    private readonly AppSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="HeartbeatBackgroundService"/> class.
    /// </summary>
    /// <param name="settings">The application settings.</param>
    /// <param name="runtimeState">The runtime state store.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="applicationLifetime">The host application lifetime.</param>
    public HeartbeatBackgroundService(
        AppSettings settings,
        AppRuntimeState runtimeState,
        ILogger<HeartbeatBackgroundService> logger,
        IHostApplicationLifetime applicationLifetime)
    {
        _settings = settings;
        _runtimeState = runtimeState;
        _logger = logger;
        _applicationLifetime = applicationLifetime;
    }

    /// <inheritdoc />
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _applicationLifetime.ApplicationStarted.Register(() =>
        {
            _logger.LogInformation("HTTP service started. Browse the root page or health endpoints to verify the app.");
        });

        _applicationLifetime.ApplicationStopping.Register(() =>
        {
            _logger.LogInformation("Shutdown signal received. Stopping HTTP service...");
        });

        _applicationLifetime.ApplicationStopped.Register(() =>
        {
            _logger.LogInformation("Cleanup completed. Goodbye.");
        });

        if (!_settings.IsLongRunning)
        {
            _logger.LogInformation("Heartbeat worker is disabled because APP_MODE is set to {RunMode}.", _settings.RunMode);
            return Task.CompletedTask;
        }

        _logger.LogInformation(
            "Heartbeat worker enabled. Mode: {RunMode}. Interval: {HeartbeatSeconds} second(s).",
            _settings.RunMode,
            _settings.HeartbeatSeconds);

        return RunHeartbeatLoopAsync(stoppingToken);
    }

    private async Task RunHeartbeatLoopAsync(CancellationToken stoppingToken)
    {
        int tick = 1;

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime heartbeatAt = DateTime.UtcNow;
                _runtimeState.RecordHeartbeat(heartbeatAt);
                _logger.LogInformation("Heartbeat {Tick}: {HeartbeatAt:yyyy-MM-dd HH:mm:ss} UTC", tick, heartbeatAt);
                tick++;
                await Task.Delay(TimeSpan.FromSeconds(_settings.HeartbeatSeconds), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}