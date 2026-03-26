using MyFirstApp.Business.Models.Queries;
using MyFirstApp.Business.Models.Validation;

namespace MyFirstApp.Business.Interfaces;

/// <summary>
/// Validates application profile query parameters.
/// </summary>
public interface IApplicationProfileQueryValidator
{
    /// <summary>
    /// Validates the search query.
    /// </summary>
    /// <param name="query">The query parameters.</param>
    /// <returns>The validation result.</returns>
    ValidationResultDto Validate(ApplicationProfileQueryDto query);

    /// <summary>
    /// Validates the profile key route value.
    /// </summary>
    /// <param name="profileKey">The profile key.</param>
    /// <returns>The validation result.</returns>
    ValidationResultDto ValidateProfileKey(string profileKey);
}
