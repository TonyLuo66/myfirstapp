using MyFirstApp.Business.Interfaces;
using MyFirstApp.Business.Models.Queries;
using MyFirstApp.Business.Models.Validation;
using MyFirstApp.Common.Constants;

namespace MyFirstApp.Business.Validators;

/// <summary>
/// Validates system status queries.
/// </summary>
public sealed class SystemStatusQueryValidator : ISystemStatusQueryValidator
{
    /// <inheritdoc />
    public ValidationResultDto Validate(SystemStatusQueryDto query)
    {
        if (query is null)
        {
	        return ValidationResultDto.Failure(ReturnCodeConstants.ValidationError, "查詢參數不可為空。");
        }

        if (query.ResponseMode is not ("summary" or "detail"))
        {
            return ValidationResultDto.Failure(ReturnCodeConstants.InvalidQueryParameter, "ResponseMode 僅允許 summary 或 detail。");
        }

        return ValidationResultDto.Success();
    }
}
