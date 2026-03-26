using MyFirstApp.Business.Models.Dtos;
using MyFirstApp.Business.Models.Commands;
using MyFirstApp.Business.Models.Queries;
using MyFirstApp.Common.Models;

namespace MyFirstApp.Business.Interfaces;

/// <summary>
/// Provides application profile use cases.
/// </summary>
public interface IApplicationProfileService
{
    /// <summary>
    /// Searches application profiles.
    /// </summary>
    /// <param name="query">The query parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The paged result.</returns>
    Task<PagedResult<ApplicationProfileDto>> SearchAsync(ApplicationProfileQueryDto query, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a single application profile by key.
    /// </summary>
    /// <param name="profileKey">The unique profile key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The application profile.</returns>
    Task<ApplicationProfileDto> GetByKeyAsync(string profileKey, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new application profile.
    /// </summary>
    /// <param name="command">The create command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created application profile.</returns>
    Task<ApplicationProfileDto> CreateAsync(CreateApplicationProfileCommand command, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing application profile.
    /// </summary>
    /// <param name="command">The update command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated application profile.</returns>
    Task<ApplicationProfileDto> UpdateAsync(UpdateApplicationProfileCommand command, CancellationToken cancellationToken);
}

