using MyFirstApp.Business.Models.Dtos;
using MyFirstApp.Business.Models.Queries;
using MyFirstApp.Common.Models;

namespace MyFirstApp.Business.Interfaces;

/// <summary>
/// Provides read-only database access for application profiles.
/// </summary>
public interface IApplicationProfileQueryRepository
{
    /// <summary>
    /// Searches application profiles using Dapper.
    /// </summary>
    /// <param name="query">The query parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The paged result.</returns>
    Task<PagedResult<ApplicationProfileDto>> SearchAsync(ApplicationProfileQueryDto query, CancellationToken cancellationToken);

    /// <summary>
    /// Gets an application profile by key.
    /// </summary>
    /// <param name="profileKey">The unique profile key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The record when found; otherwise, null.</returns>
    Task<ApplicationProfileDto?> GetByKeyAsync(string profileKey, CancellationToken cancellationToken);
}
