namespace MyFirstApp.Business.Models.Dtos;

/// <summary>
/// Represents an application profile record retrieved from the database.
/// </summary>
public sealed class ApplicationProfileDto
{
    /// <summary>
    /// Gets or sets the unique profile key.
    /// </summary>
    public required string ProfileKey { get; init; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Gets or sets the owner team name.
    /// </summary>
    public required string OwnerTeam { get; init; }

    /// <summary>
    /// Gets or sets the target environment.
    /// </summary>
    public required string Environment { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the profile is active.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Gets or sets the latest update timestamp.
    /// </summary>
    public DateTime UpdatedAt { get; init; }
}
