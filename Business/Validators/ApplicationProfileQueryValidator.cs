using MyFirstApp.Business.Interfaces;
using MyFirstApp.Business.Models.Queries;
using MyFirstApp.Business.Models.Validation;
using MyFirstApp.Common.Constants;
using System.Text.RegularExpressions;

namespace MyFirstApp.Business.Validators;

/// <summary>
/// Validates application profile queries.
/// </summary>
public sealed class ApplicationProfileQueryValidator : IApplicationProfileQueryValidator
{
    private static readonly Regex ProfileKeyPattern = new("^[a-z0-9-]{2,40}$", RegexOptions.Compiled);

    /// <inheritdoc />
    public ValidationResultDto Validate(ApplicationProfileQueryDto query)
    {
        if (query is null)
        {
            return ValidationResultDto.Failure(ReturnCodeConstants.ValidationError, "查詢參數不可為空。");
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            string keyword = query.Keyword.Trim();
            if (keyword.Length < 2)
            {
                return ValidationResultDto.Failure(ReturnCodeConstants.InvalidQueryParameter, "Keyword 長度至少需要 2 個字元。");
            }

            if (keyword.Length > 50)
            {
                return ValidationResultDto.Failure(ReturnCodeConstants.InvalidQueryParameter, "Keyword 長度不得超過 50 個字元。");
            }
        }

        if (query.PageNumber <= 0)
        {
            return ValidationResultDto.Failure(ReturnCodeConstants.InvalidQueryParameter, "PageNumber 必須大於 0。");
        }

        if (query.PageSize <= 0 || query.PageSize > 100)
        {
            return ValidationResultDto.Failure(ReturnCodeConstants.InvalidQueryParameter, "PageSize 必須介於 1 到 100 之間。");
        }

        return ValidationResultDto.Success();
    }

    /// <inheritdoc />
    public ValidationResultDto ValidateProfileKey(string profileKey)
    {
        if (string.IsNullOrWhiteSpace(profileKey))
        {
            return ValidationResultDto.Failure(ReturnCodeConstants.InvalidQueryParameter, "profileKey 不可為空。");
        }

        if (!ProfileKeyPattern.IsMatch(profileKey.Trim()))
        {
            return ValidationResultDto.Failure(ReturnCodeConstants.InvalidQueryParameter, "profileKey 僅允許小寫英數與連字號，長度需介於 2 到 40。");
        }

        return ValidationResultDto.Success();
    }
}
