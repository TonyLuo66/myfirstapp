using Microsoft.AspNetCore.Http;
using MyFirstApp.Business.Interfaces;
using MyFirstApp.Business.Models.Commands;
using MyFirstApp.Business.Models.Dtos;
using MyFirstApp.Business.Models.Queries;
using MyFirstApp.Common.Constants;
using MyFirstApp.Common.Exceptions;
using MyFirstApp.Common.Models;

namespace MyFirstApp.Business.Services;

/// <summary>
/// Implements application profile queries using the repository pattern.
/// </summary>
public sealed class ApplicationProfileService : IApplicationProfileService
{
    private readonly IApplicationProfileCommandRepository _commandRepository;
    private readonly IApplicationProfileQueryRepository _queryRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationProfileService"/> class.
    /// </summary>
    /// <param name="queryRepository">The application profile query repository.</param>
    /// <param name="commandRepository">The application profile command repository.</param>
    public ApplicationProfileService(IApplicationProfileQueryRepository queryRepository, IApplicationProfileCommandRepository commandRepository)
    {
        _queryRepository = queryRepository;
        _commandRepository = commandRepository;
    }

    /// <inheritdoc />
    public Task<PagedResult<ApplicationProfileDto>> SearchAsync(ApplicationProfileQueryDto query, CancellationToken cancellationToken)
    {
        return _queryRepository.SearchAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ApplicationProfileDto> GetByKeyAsync(string profileKey, CancellationToken cancellationToken)
    {
        string normalizedProfileKey = profileKey.Trim();
        ApplicationProfileDto? profile = await _queryRepository.GetByKeyAsync(normalizedProfileKey, cancellationToken);
        if (profile is null)
        {
            throw new AppException(StatusCodes.Status404NotFound, ReturnCodeConstants.RecordNotFound, $"查無對應資料: {normalizedProfileKey}");
        }

        return profile;
    }

    /// <inheritdoc />
    public async Task<ApplicationProfileDto> CreateAsync(CreateApplicationProfileCommand command, CancellationToken cancellationToken)
    {
        CreateApplicationProfileCommand normalizedCommand = NormalizeCreateCommand(command);
        await _commandRepository.CreateAsync(normalizedCommand, cancellationToken);
        return await GetByKeyAsync(normalizedCommand.ProfileKey, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ApplicationProfileDto> UpdateAsync(UpdateApplicationProfileCommand command, CancellationToken cancellationToken)
    {
        UpdateApplicationProfileCommand normalizedCommand = NormalizeUpdateCommand(command);
        int affectedRows = await _commandRepository.UpdateAsync(normalizedCommand, cancellationToken);
        if (affectedRows == 0)
        {
            throw new AppException(StatusCodes.Status404NotFound, ReturnCodeConstants.RecordNotFound, $"查無對應資料: {normalizedCommand.ProfileKey}");
        }

        return await GetByKeyAsync(normalizedCommand.ProfileKey, cancellationToken);
    }

    private static CreateApplicationProfileCommand NormalizeCreateCommand(CreateApplicationProfileCommand command)
    {
        return new CreateApplicationProfileCommand
        {
            ProfileKey = command.ProfileKey.Trim(),
            DisplayName = command.DisplayName.Trim(),
            OwnerTeam = command.OwnerTeam.Trim(),
            Environment = command.Environment.Trim(),
            IsActive = command.IsActive
        };
    }

    private static UpdateApplicationProfileCommand NormalizeUpdateCommand(UpdateApplicationProfileCommand command)
    {
        return new UpdateApplicationProfileCommand
        {
            ProfileKey = command.ProfileKey.Trim(),
            DisplayName = command.DisplayName.Trim(),
            OwnerTeam = command.OwnerTeam.Trim(),
            Environment = command.Environment.Trim(),
            IsActive = command.IsActive
        };
    }
}
