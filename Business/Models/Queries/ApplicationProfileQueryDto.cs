namespace MyFirstApp.Business.Models.Queries;

/// <summary>
/// Represents query parameters for searching application profiles.
/// </summary>
public sealed class ApplicationProfileQueryDto
{
    /// <summary>
    /// Gets or sets the optional keyword matched against profile key and display name.
    /// </summary>
    public string? Keyword { get; init; }

    /// <summary>
    /// Gets or sets the optional active-state filter.
    /// </summary>
    public bool? IsActive { get; init; }

    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    public int PageSize { get; init; } = 20;
}
