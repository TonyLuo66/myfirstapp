using MyFirstApp.Business.Interfaces;
using MyFirstApp.Business.Models.Commands;
using MyFirstApp.Business.Models.Validation;
using MyFirstApp.Common.Constants;
using System.Text.RegularExpressions;

namespace MyFirstApp.Business.Validators;

/// <summary>
/// Validates application profile create and update commands.
/// </summary>
public sealed class ApplicationProfileCommandValidator : IApplicationProfileCommandValidator
{
    private static readonly Regex ProfileKeyPattern = new("^[a-z0-9-]{2,40}$", RegexOptions.Compiled);

    /// <inheritdoc />
    public ValidationResultDto ValidateCreate(CreateApplicationProfileCommand command)
    {
        if (command is null)
        {
            return ValidationResultDto.Failure(ReturnCodeConstants.ValidationError, "新增資料不可為空。");
        }

        return ValidateCore(command.ProfileKey, command.DisplayName, command.OwnerTeam, command.Environment);
    }

    /// <inheritdoc />
    public ValidationResultDto ValidateUpdate(UpdateApplicationProfileCommand command)
    {
        if (command is null)
        {
            return ValidationResultDto.Failure(ReturnCodeConstants.ValidationError, "更新資料不可為空。");
        }

        return ValidateCore(command.ProfileKey, command.DisplayName, command.OwnerTeam, command.Environment);
    }

    private static ValidationResultDto ValidateCore(string profileKey, string displayName, string ownerTeam, string environment)
    {
        if (string.IsNullOrWhiteSpace(profileKey) || !ProfileKeyPattern.IsMatch(profileKey.Trim()))
        {
            return ValidationResultDto.Failure(ReturnCodeConstants.InvalidQueryParameter, "profileKey 僅允許小寫英數與連字號，長度需介於 2 到 40。");
        }

        if (string.IsNullOrWhiteSpace(displayName) || displayName.Trim().Length > 100)
        {
            return ValidationResultDto.Failure(ReturnCodeConstants.InvalidQueryParameter, "DisplayName 為必填且長度不得超過 100。");
        }

        if (string.IsNullOrWhiteSpace(ownerTeam) || ownerTeam.Trim().Length > 100)
        {
            return ValidationResultDto.Failure(ReturnCodeConstants.InvalidQueryParameter, "OwnerTeam 為必填且長度不得超過 100。");
        }

        string[] allowedEnvironments = ["Development", "SIT", "UAT", "Production", "local"];
        if (string.IsNullOrWhiteSpace(environment) || !allowedEnvironments.Contains(environment.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            return ValidationResultDto.Failure(ReturnCodeConstants.InvalidQueryParameter, "Environment 僅允許 Development、SIT、UAT、Production、local。");
        }

        return ValidationResultDto.Success();
    }
}