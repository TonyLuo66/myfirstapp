namespace MyFirstApp.Business.Models.Validation;

/// <summary>
/// Represents the result of business validation.
/// </summary>
public sealed class ValidationResultDto
{
    private ValidationResultDto(bool isValid, string? errorCode, string? errorMessage)
    {
        IsValid = isValid;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Gets a value indicating whether the validation succeeded.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Gets the validation error message when validation fails.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets the validation error code when validation fails.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    /// <returns>The success result.</returns>
    public static ValidationResultDto Success()
    {
        return new ValidationResultDto(true, null, null);
    }

    /// <summary>
    /// Creates a failed validation result.
    /// </summary>
    /// <param name="errorCode">The validation error code.</param>
    /// <param name="errorMessage">The validation message.</param>
    /// <returns>The failure result.</returns>
    public static ValidationResultDto Failure(string errorCode, string errorMessage)
    {
        return new ValidationResultDto(false, errorCode, errorMessage);
    }
}
