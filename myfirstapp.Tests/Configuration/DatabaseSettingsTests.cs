using Microsoft.Extensions.Configuration;
using MyFirstApp.Configuration;

namespace MyFirstApp.Tests.Configuration;

public sealed class DatabaseSettingsTests
{
    [Fact]
    public void TryCreate_UsesDefaultsWhenOptionalFlagsAreMissing()
    {
        IConfiguration configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Server=test;Database=myfirstapp;",
            ["ConnectionStrings:MasterConnection"] = "Server=test;Database=master;",
            ["Database:SeedOnStartup"] = "false"
        });

        bool success = DatabaseSettings.TryCreate(configuration, out DatabaseSettings? settings, out string? errorMessage);

        Assert.True(success);
        Assert.Null(errorMessage);
        Assert.NotNull(settings);
        Assert.False(settings.SeedOnStartup);
        Assert.True(settings.InitializeOnStartup);
        Assert.True(settings.CreateDatabaseIfMissing);
    }

    [Fact]
    public void TryCreate_ReadsStartupInitializationFlags()
    {
        IConfiguration configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Server=test;Database=myfirstapp;",
            ["ConnectionStrings:MasterConnection"] = "Server=test;Database=master;",
            ["DATABASE_INITIALIZE_ON_STARTUP"] = "false",
            ["DATABASE_CREATE_IF_MISSING"] = "false"
        });

        bool success = DatabaseSettings.TryCreate(configuration, out DatabaseSettings? settings, out string? errorMessage);

        Assert.True(success);
        Assert.Null(errorMessage);
        Assert.NotNull(settings);
        Assert.False(settings.InitializeOnStartup);
        Assert.False(settings.CreateDatabaseIfMissing);
    }

    [Fact]
    public void TryCreate_ReturnsErrorForInvalidCreateDatabaseFlag()
    {
        IConfiguration configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Server=test;Database=myfirstapp;",
            ["DATABASE_CREATE_IF_MISSING"] = "sometimes"
        });

        bool success = DatabaseSettings.TryCreate(configuration, out DatabaseSettings? settings, out string? errorMessage);

        Assert.False(success);
        Assert.Null(settings);
        Assert.Equal("Database:CreateDatabaseIfMissing 必須是 true 或 false。", errorMessage);
    }

    private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }
}
