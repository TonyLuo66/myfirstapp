namespace MyFirstApp.Business.Models.Commands;

/// <summary>
/// Represents an update command for an application profile.
/// </summary>
public sealed class UpdateApplicationProfileCommand
{
    /// <summary>
    /// Gets or sets the unique profile key.
    /// </summary>
    public string ProfileKey { get; init; } = string.Empty;

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
}
