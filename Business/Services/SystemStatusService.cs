using MyFirstApp.Business.Interfaces;
using MyFirstApp.Business.Models.Dtos;
using MyFirstApp.Business.Models.Queries;
using MyFirstApp.Configuration;
using MyFirstApp.Infrastructure.Runtime;

namespace MyFirstApp.Business.Services;

/// <summary>
/// Default implementation for service status queries.
/// </summary>
public sealed class SystemStatusService : ISystemStatusService
{
    private readonly AppRuntimeState _runtimeState;
    private readonly AppSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemStatusService"/> class.
    /// </summary>
    /// <param name="settings">The application settings.</param>
    /// <param name="runtimeState">The runtime state store.</param>
    public SystemStatusService(AppSettings settings, AppRuntimeState runtimeState)
    {
        _settings = settings;
        _runtimeState = runtimeState;
    }

    /// <inheritdoc />
    public HealthStatusDto GetHealthStatus()
    {
        AppRuntimeSnapshot snapshot = _runtimeState.CreateSnapshot();

        return new HealthStatusDto
        {
            Status = "ok",
            AppName = _settings.AppName,
            AppEnvironment = _settings.AppEnvironment,
            RunMode = _settings.RunMode,
            LastHeartbeatAt = snapshot.LastHeartbeatAt,
            CheckedAt = DateTime.UtcNow
        };
    }

    /// <inheritdoc />
    public SystemStatusDto GetSystemStatus(SystemStatusQueryDto query)
    {
        AppRuntimeSnapshot snapshot = _runtimeState.CreateSnapshot();

        return new SystemStatusDto
        {
            AppName = _settings.AppName,
            AppEnvironment = _settings.AppEnvironment,
            RunMode = _settings.RunMode,
            RunModeSource = _settings.RunModeSource,
            HeartbeatSeconds = _settings.HeartbeatSeconds,
            HeartbeatCount = snapshot.HeartbeatCount,
            LastHeartbeatAt = snapshot.LastHeartbeatAt,
            AppMessage = query.IncludeDetails ? _settings.AppMessage : null,
            ReportedAt = DateTime.UtcNow
        };
    }
}
