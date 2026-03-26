using Microsoft.AspNetCore.Mvc;
using MyFirstApp.Business.Interfaces;
using MyFirstApp.Business.Models.Dtos;
using MyFirstApp.Business.Models.Queries;
using MyFirstApp.Common.Models;

namespace MyFirstApp.Controllers;

/// <summary>
/// Provides system status endpoints.
/// </summary>
[Route("api/system-status")]
public sealed class SystemStatusController : BaseApiController
{
    private readonly ISystemStatusQueryValidator _queryValidator;
    private readonly ISystemStatusService _systemStatusService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemStatusController"/> class.
    /// </summary>
    /// <param name="systemStatusService">The system status service.</param>
    /// <param name="queryValidator">The query validator.</param>
    public SystemStatusController(
        ISystemStatusService systemStatusService,
        ISystemStatusQueryValidator queryValidator)
    {
        _systemStatusService = systemStatusService;
        _queryValidator = queryValidator;
    }

    /// <summary>
    /// Returns a simple health check result.
    /// </summary>
    /// <returns>The wrapped health check response.</returns>
    [HttpGet("health")]
    [ProducesResponseType(typeof(ApiResponse<HealthStatusDto>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<HealthStatusDto>> GetHealth()
    {
        HealthStatusDto result = _systemStatusService.GetHealthStatus();
        return Success(result);
    }

    /// <summary>
    /// Returns the current runtime status.
    /// </summary>
    /// <param name="query">The query parameters.</param>
    /// <returns>The wrapped system status response.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<SystemStatusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SystemStatusDto>), StatusCodes.Status400BadRequest)]
    public ActionResult<ApiResponse<SystemStatusDto>> GetStatus([FromQuery] SystemStatusQueryDto query)
    {
        var validationResult = _queryValidator.Validate(query);
        if (!validationResult.IsValid)
        {
            return Failure<SystemStatusDto>(
                StatusCodes.Status400BadRequest,
                validationResult.ErrorCode ?? MyFirstApp.Common.Constants.ReturnCodeConstants.ValidationError,
                validationResult.ErrorMessage ?? "查詢參數驗證失敗。");
        }

        SystemStatusDto result = _systemStatusService.GetSystemStatus(query);
        return Success(result);
    }
}
