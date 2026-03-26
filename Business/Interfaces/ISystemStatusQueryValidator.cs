using MyFirstApp.Business.Models.Queries;
using MyFirstApp.Business.Models.Validation;

namespace MyFirstApp.Business.Interfaces;

/// <summary>
/// Validates system status query input.
/// </summary>
public interface ISystemStatusQueryValidator
{
    /// <summary>
    /// Validates the given system status query.
    /// </summary>
    /// <param name="query">The query model.</param>
    /// <returns>The validation result.</returns>
    ValidationResultDto Validate(SystemStatusQueryDto query);
}
