using Microsoft.AspNetCore.Mvc;
using MyFirstApp.Business.Interfaces;
using MyFirstApp.Business.Models.Commands;
using MyFirstApp.Business.Models.Dtos;
using MyFirstApp.Business.Models.Queries;
using MyFirstApp.Common.Constants;
using MyFirstApp.Common.Models;

namespace MyFirstApp.Controllers;

/// <summary>
/// Provides Dapper-backed application profile endpoints.
/// </summary>
[Route("api/application-profiles")]
public sealed class ApplicationProfilesController : BaseApiController
{
    private readonly IApplicationProfileCommandValidator _commandValidator;
    private readonly IApplicationProfileQueryValidator _validator;
    private readonly IApplicationProfileService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationProfilesController"/> class.
    /// </summary>
    /// <param name="service">The application profile service.</param>
    /// <param name="validator">The query validator.</param>
    /// <param name="commandValidator">The command validator.</param>
    public ApplicationProfilesController(IApplicationProfileService service, IApplicationProfileQueryValidator validator, IApplicationProfileCommandValidator commandValidator)
    {
        _service = service;
        _validator = validator;
        _commandValidator = commandValidator;
    }

    /// <summary>
    /// Searches application profiles stored in SQL Server.
    /// </summary>
    /// <param name="query">The query parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The paged search result.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ApplicationProfileDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ApplicationProfileDto>>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PagedResult<ApplicationProfileDto>>>> Search([FromQuery] ApplicationProfileQueryDto query, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(query);
        if (!validationResult.IsValid)
        {
            return Failure<PagedResult<ApplicationProfileDto>>(
                StatusCodes.Status400BadRequest,
                validationResult.ErrorCode ?? ReturnCodeConstants.ValidationError,
                validationResult.ErrorMessage ?? "查詢參數驗證失敗。");
        }

        PagedResult<ApplicationProfileDto> result = await _service.SearchAsync(query, cancellationToken);
        return Success(result);
    }

    /// <summary>
    /// Gets a single application profile by key.
    /// </summary>
    /// <param name="profileKey">The profile key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The application profile record.</returns>
    [HttpGet("{profileKey}")]
    [ProducesResponseType(typeof(ApiResponse<ApplicationProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ApplicationProfileDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ApplicationProfileDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ApplicationProfileDto>>> GetByKey(string profileKey, CancellationToken cancellationToken)
    {
        var validationResult = _validator.ValidateProfileKey(profileKey);
        if (!validationResult.IsValid)
        {
            return Failure<ApplicationProfileDto>(
                StatusCodes.Status400BadRequest,
                validationResult.ErrorCode ?? ReturnCodeConstants.ValidationError,
                validationResult.ErrorMessage ?? "profileKey 驗證失敗。");
        }

        ApplicationProfileDto result = await _service.GetByKeyAsync(profileKey.Trim(), cancellationToken);
        return Success(result);
    }

    /// <summary>
    /// Creates a new application profile.
    /// </summary>
    /// <param name="command">The create command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created application profile.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ApplicationProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ApplicationProfileDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ApplicationProfileDto>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<ApplicationProfileDto>>> Create([FromBody] CreateApplicationProfileCommand command, CancellationToken cancellationToken)
    {
        var validationResult = _commandValidator.ValidateCreate(command);
        if (!validationResult.IsValid)
        {
            return Failure<ApplicationProfileDto>(StatusCodes.Status400BadRequest, validationResult.ErrorCode ?? ReturnCodeConstants.ValidationError, validationResult.ErrorMessage ?? "新增資料驗證失敗。");
        }

        ApplicationProfileDto result = await _service.CreateAsync(command, cancellationToken);
        return Success(result, "created");
    }

    /// <summary>
    /// Updates an existing application profile.
    /// </summary>
    /// <param name="profileKey">The route profile key.</param>
    /// <param name="command">The update command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated application profile.</returns>
    [HttpPut("{profileKey}")]
    [ProducesResponseType(typeof(ApiResponse<ApplicationProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ApplicationProfileDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ApplicationProfileDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ApplicationProfileDto>>> Update(string profileKey, [FromBody] UpdateApplicationProfileCommand command, CancellationToken cancellationToken)
    {
        UpdateApplicationProfileCommand request = new()
        {
            ProfileKey = profileKey,
            DisplayName = command.DisplayName,
            OwnerTeam = command.OwnerTeam,
            Environment = command.Environment,
            IsActive = command.IsActive
        };

        var validationResult = _commandValidator.ValidateUpdate(request);
        if (!validationResult.IsValid)
        {
            return Failure<ApplicationProfileDto>(StatusCodes.Status400BadRequest, validationResult.ErrorCode ?? ReturnCodeConstants.ValidationError, validationResult.ErrorMessage ?? "更新資料驗證失敗。");
        }

        ApplicationProfileDto result = await _service.UpdateAsync(request, cancellationToken);
        return Success(result, "updated");
    }
}