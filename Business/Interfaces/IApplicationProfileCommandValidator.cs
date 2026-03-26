using MyFirstApp.Business.Models.Commands;
using MyFirstApp.Business.Models.Validation;

namespace MyFirstApp.Business.Interfaces;

/// <summary>
/// Validates application profile create and update commands.
/// </summary>
public interface IApplicationProfileCommandValidator
{
    /// <summary>
    /// Validates the create command.
    /// </summary>
    /// <param name="command">The create command.</param>
    /// <returns>The validation result.</returns>
    ValidationResultDto ValidateCreate(CreateApplicationProfileCommand command);

    /// <summary>
    /// Validates the update command.
    /// </summary>
    /// <param name="command">The update command.</param>
    /// <returns>The validation result.</returns>
    ValidationResultDto ValidateUpdate(UpdateApplicationProfileCommand command);
}
