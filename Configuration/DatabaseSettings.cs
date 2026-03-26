using Microsoft.Extensions.Configuration;

namespace MyFirstApp.Configuration;

/// <summary>
/// Represents database-related configuration.
/// </summary>
public sealed class DatabaseSettings
{
    private DatabaseSettings(
        string connectionString,
        bool seedOnStartup,
        string masterConnectionString,
        bool initializeOnStartup,
        bool createDatabaseIfMissing)
    {
        ConnectionString = connectionString;
        SeedOnStartup = seedOnStartup;
        MasterConnectionString = masterConnectionString;
        InitializeOnStartup = initializeOnStartup;
        CreateDatabaseIfMissing = createDatabaseIfMissing;
    }

    /// <summary>
    /// Gets the SQL Server application connection string.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    /// Gets the SQL Server master connection string used for bootstrap operations.
    /// </summary>
    public string MasterConnectionString { get; }

    /// <summary>
    /// Gets a value indicating whether seed data should be inserted on startup.
    /// </summary>
    public bool SeedOnStartup { get; }

    /// <summary>
    /// Gets a value indicating whether startup should run database initialization.
    /// </summary>
    public bool InitializeOnStartup { get; }

    /// <summary>
    /// Gets a value indicating whether startup should create the database when it is missing.
    /// </summary>
    public bool CreateDatabaseIfMissing { get; }

    /// <summary>
    /// Builds a validated <see cref="DatabaseSettings"/> instance from layered configuration.
    /// </summary>
    /// <param name="configuration">The application configuration root.</param>
    /// <param name="settings">The parsed settings instance when parsing succeeds.</param>
    /// <param name="errorMessage">The validation error message when parsing fails.</param>
    /// <returns><c>true</c> when parsing succeeds; otherwise, <c>false</c>.</returns>
    public static bool TryCreate(IConfiguration configuration, out DatabaseSettings? settings, out string? errorMessage)
    {
        string? connectionString = configuration["DB_CONNECTION_STRING"] ?? configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            settings = null;
            errorMessage = "資料庫連線字串未設定。請提供 ConnectionStrings:DefaultConnection 或 DB_CONNECTION_STRING。";
            return false;
        }

        string? masterConnectionString = configuration["DB_MASTER_CONNECTION_STRING"] ?? configuration.GetConnectionString("MasterConnection");
        if (string.IsNullOrWhiteSpace(masterConnectionString))
        {
            masterConnectionString = connectionString;
        }

        string? seedOnStartupText = configuration["DATABASE_SEED_ON_STARTUP"] ?? configuration["Database:SeedOnStartup"];
        bool seedOnStartup = true;
        if (!string.IsNullOrWhiteSpace(seedOnStartupText) && !bool.TryParse(seedOnStartupText, out seedOnStartup))
        {
            settings = null;
            errorMessage = "Database:SeedOnStartup 必須是 true 或 false。";
            return false;
        }

        string? initializeOnStartupText = configuration["DATABASE_INITIALIZE_ON_STARTUP"] ?? configuration["Database:InitializeOnStartup"];
        bool initializeOnStartup = true;
        if (!string.IsNullOrWhiteSpace(initializeOnStartupText) && !bool.TryParse(initializeOnStartupText, out initializeOnStartup))
        {
            settings = null;
            errorMessage = "Database:InitializeOnStartup 必須是 true 或 false。";
            return false;
        }

        string? createDatabaseIfMissingText = configuration["DATABASE_CREATE_IF_MISSING"] ?? configuration["Database:CreateDatabaseIfMissing"];
        bool createDatabaseIfMissing = true;
        if (!string.IsNullOrWhiteSpace(createDatabaseIfMissingText) && !bool.TryParse(createDatabaseIfMissingText, out createDatabaseIfMissing))
        {
            settings = null;
            errorMessage = "Database:CreateDatabaseIfMissing 必須是 true 或 false。";
            return false;
        }

        settings = new DatabaseSettings(
            connectionString,
            seedOnStartup,
            masterConnectionString,
            initializeOnStartup,
            createDatabaseIfMissing);
        errorMessage = null;
        return true;
    }
}
