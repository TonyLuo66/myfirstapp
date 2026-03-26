namespace MyFirstApp.Common.Models;

/// <summary>
/// Represents a paged query result.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public sealed class PagedResult<TItem>
{
    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Gets or sets the total record count.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Gets or sets the result items.
    /// </summary>
    public required IReadOnlyList<TItem> Items { get; init; }
}
