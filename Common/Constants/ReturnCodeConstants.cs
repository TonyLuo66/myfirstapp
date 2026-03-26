namespace MyFirstApp.Common.Constants;

/// <summary>
/// Defines standard API return codes.
/// </summary>
public static class ReturnCodeConstants
{
    /// <summary>
    /// Indicates the request completed successfully.
    /// </summary>
    public const string Success = "0000";

    /// <summary>
    /// Indicates a validation failure.
    /// </summary>
    public const string ValidationError = "1001";

    /// <summary>
    /// Indicates an invalid query parameter or paging argument.
    /// </summary>
    public const string InvalidQueryParameter = "1002";

    /// <summary>
    /// Indicates the requested record was not found.
    /// </summary>
    public const string RecordNotFound = "1404";

    /// <summary>
    /// Indicates the target record already exists.
    /// </summary>
    public const string RecordAlreadyExists = "1409";

    /// <summary>
    /// Indicates a database access failure.
    /// </summary>
    public const string DatabaseError = "2001";

    /// <summary>
    /// Indicates an unexpected system error.
    /// </summary>
    public const string SystemError = "9999";
}
