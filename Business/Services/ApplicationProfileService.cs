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
        ApplicationProfileDto? profile = await _queryRepository.GetByKeyAsync(profileKey, cancellationToken);
        if (profile is null)
        {
            throw new AppException(StatusCodes.Status404NotFound, ReturnCodeConstants.RecordNotFound, $"查無對應資料: {profileKey}");
        }

        return profile;
    }

    /// <inheritdoc />
    public async Task<ApplicationProfileDto> CreateAsync(CreateApplicationProfileCommand command, CancellationToken cancellationToken)
    {
        await _commandRepository.CreateAsync(command, cancellationToken);
        return await GetByKeyAsync(command.ProfileKey, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ApplicationProfileDto> UpdateAsync(UpdateApplicationProfileCommand command, CancellationToken cancellationToken)
    {
        int affectedRows = await _commandRepository.UpdateAsync(command, cancellationToken);
        if (affectedRows == 0)
        {
            throw new AppException(StatusCodes.Status404NotFound, ReturnCodeConstants.RecordNotFound, $"查無對應資料: {command.ProfileKey}");
        }

        return await GetByKeyAsync(command.ProfileKey, cancellationToken);
    }
}
