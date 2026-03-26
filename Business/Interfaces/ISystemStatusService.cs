using MyFirstApp.Business.Models.Dtos;
using MyFirstApp.Business.Models.Queries;

namespace MyFirstApp.Business.Interfaces;

/// <summary>
/// Provides system status information.
/// </summary>
public interface ISystemStatusService
{
    /// <summary>
    /// Builds the service health response.
    /// </summary>
    /// <returns>The health status payload.</returns>
    HealthStatusDto GetHealthStatus();

    /// <summary>
    /// Builds the service runtime status response.
    /// </summary>
    /// <param name="query">The query model.</param>
    /// <returns>The runtime status payload.</returns>
    SystemStatusDto GetSystemStatus(SystemStatusQueryDto query);
}
