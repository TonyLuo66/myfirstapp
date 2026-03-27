using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Console;
using Microsoft.OpenApi.Models;
using MyFirstApp.BackgroundServices;
using MyFirstApp.Business.Interfaces;
using MyFirstApp.Business.Models.Dtos;
using MyFirstApp.Business.Models.Queries;
using MyFirstApp.Business.Services;
using MyFirstApp.Business.Validators;
using MyFirstApp.Common.Constants;
using MyFirstApp.Common.Models;
using MyFirstApp.Configuration;
using MyFirstApp.Infrastructure.Data;
using MyFirstApp.Infrastructure.Presentation;
using MyFirstApp.Infrastructure.Repositories;
using MyFirstApp.Infrastructure.Runtime;
using MyFirstApp.Middleware;
using System.Reflection;

ILoggerFactory bootstrapLoggerFactory = LoggerFactory.Create(logging =>
{
    logging.ClearProviders();
    logging.AddSimpleConsole(options =>
    {
        options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
        options.SingleLine = true;
        options.IncludeScopes = false;
        options.ColorBehavior = LoggerColorBehavior.Disabled;
    });
});

ILogger bootstrapLogger = bootstrapLoggerFactory.CreateLogger("Startup");

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    builder.Configuration.AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true);
    builder.Logging.ClearProviders();
    builder.Logging.AddSimpleConsole(options =>
    {
        options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
        options.SingleLine = true;
        options.IncludeScopes = false;
        options.ColorBehavior = LoggerColorBehavior.Disabled;
    });

    if (!AppSettings.TryCreate(builder.Configuration, builder.Environment.EnvironmentName, args, out AppSettings? settings, out string? errorMessage))
    {
        bootstrapLogger.LogCritical("Application settings validation failed during startup: {ErrorMessage}", errorMessage);
        return;
    }

    if (!DatabaseSettings.TryCreate(builder.Configuration, out DatabaseSettings? databaseSettings, out errorMessage))
    {
        bootstrapLogger.LogCritical("Database settings validation failed during startup: {ErrorMessage}", errorMessage);
        return;
    }

    AppSettings appSettings = settings!;
    DatabaseSettings dbSettings = databaseSettings!;

    builder.Services.AddSingleton(appSettings);
    builder.Services.AddSingleton(dbSettings);
    builder.Services.AddSingleton<AppRuntimeState>();
    builder.Services.AddSingleton<DatabaseInitializer>();
    builder.Services.AddSingleton<ISqlConnectionFactory, SqlServerConnectionFactory>();
    builder.Services.AddScoped<IApplicationProfileQueryRepository, ApplicationProfileQueryRepository>();
    builder.Services.AddScoped<IApplicationProfileCommandRepository, ApplicationProfileCommandRepository>();
    builder.Services.AddScoped<IApplicationProfileService, ApplicationProfileService>();
    builder.Services.AddScoped<IApplicationProfileQueryValidator, ApplicationProfileQueryValidator>();
    builder.Services.AddScoped<IApplicationProfileCommandValidator, ApplicationProfileCommandValidator>();
    builder.Services.AddScoped<ISystemStatusService, SystemStatusService>();
    builder.Services.AddScoped<ISystemStatusQueryValidator, SystemStatusQueryValidator>();
    builder.Services.AddHostedService<HeartbeatBackgroundService>();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "myfirstapp API",
            Version = "v1",
            Description = "第一階段重構後的示範 Web API。"
        });

        string xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        string xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFileName);
        if (File.Exists(xmlFilePath))
        {
            options.IncludeXmlComments(xmlFilePath, includeControllerXmlComments: true);
        }
    });

    WebApplication app = builder.Build();

    using (IServiceScope scope = app.Services.CreateScope())
    {
        try
        {
            DatabaseInitializer initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
            await initializer.InitializeAsync(CancellationToken.None);
        }
        catch (Exception exception)
        {
            bootstrapLogger.LogCritical(exception, "Database initialization failed during application startup.");
            return;
        }
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseMiddleware<RequestLoggingMiddleware>();
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapGet("/", ([FromServices] ISystemStatusService systemStatusService) =>
    {
        SystemStatusDto status = systemStatusService.GetSystemStatus(new SystemStatusQueryDto());
        string html = HomePageRenderer.Render(status);
        return Results.Content(html, "text/html; charset=utf-8");
    });

    app.MapGet("/health", ([FromServices] ISystemStatusService systemStatusService) =>
    {
        HealthStatusDto result = systemStatusService.GetHealthStatus();
        return Results.Ok(new ApiResponse<HealthStatusDto>(ReturnCodeConstants.Success, "success", result));
    });

    app.MapGet("/api/status", ([FromServices] ISystemStatusService systemStatusService, [AsParameters] SystemStatusQueryDto query) =>
    {
        SystemStatusDto result = systemStatusService.GetSystemStatus(query);
        return Results.Ok(new ApiResponse<SystemStatusDto>(ReturnCodeConstants.Success, "success", result));
    });

    app.MapControllers();

    await app.RunAsync();
}
catch (Exception exception)
{
    bootstrapLogger.LogCritical(exception, "Application startup failed unexpectedly.");
}
finally
{
    bootstrapLoggerFactory.Dispose();
}