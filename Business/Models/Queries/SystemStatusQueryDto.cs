namespace MyFirstApp.Business.Models.Queries;

/// <summary>
/// Represents query parameters for the system status endpoint.
/// </summary>
public sealed class SystemStatusQueryDto
{
    /// <summary>
    /// Gets or sets a value indicating whether detailed fields should be returned.
    /// </summary>
    public bool IncludeDetails { get; init; } = true;

    /// <summary>
    /// Gets or sets the response mode. Allowed values: summary, detail.
    /// </summary>
    public string ResponseMode { get; init; } = "detail";
}
