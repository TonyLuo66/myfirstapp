using MyFirstApp.Business.Models.Commands;

namespace MyFirstApp.Business.Interfaces;

/// <summary>
/// Provides write access for application profile records.
/// </summary>
public interface IApplicationProfileCommandRepository
{
    /// <summary>
    /// Inserts a new application profile.
    /// </summary>
    /// <param name="command">The create command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that completes when the insert finishes.</returns>
    Task CreateAsync(CreateApplicationProfileCommand command, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing application profile.
    /// </summary>
    /// <param name="command">The update command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of affected rows.</returns>
    Task<int> UpdateAsync(UpdateApplicationProfileCommand command, CancellationToken cancellationToken);
}
