using Microsoft.Extensions.Configuration;

namespace MyFirstApp.Configuration;

/// <summary>
/// Represents runtime configuration sourced from environment variables and startup arguments.
/// </summary>
public sealed class AppSettings
{
	private AppSettings(
		string appName,
		string appEnvironment,
		string? appMessage,
		string runMode,
		string runModeSource,
		int heartbeatSeconds)
	{
		AppName = appName;
		AppEnvironment = appEnvironment;
		AppMessage = appMessage;
		RunMode = runMode;
		RunModeSource = runModeSource;
		HeartbeatSeconds = heartbeatSeconds;
	}

	/// <summary>
	/// Gets the application display name.
	/// </summary>
	public string AppName { get; }

	/// <summary>
	/// Gets the application environment name.
	/// </summary>
	public string AppEnvironment { get; }

	/// <summary>
	/// Gets the optional custom message.
	/// </summary>
	public string? AppMessage { get; }

	/// <summary>
	/// Gets the configured execution mode.
	/// </summary>
	public string RunMode { get; }

	/// <summary>
	/// Gets the source of the configured execution mode.
	/// </summary>
	public string RunModeSource { get; }

	/// <summary>
	/// Gets the heartbeat interval in seconds.
	/// </summary>
	public int HeartbeatSeconds { get; }

	/// <summary>
	/// Gets a value indicating whether background heartbeat processing is enabled.
	/// </summary>
	public bool IsLongRunning => RunMode is "watch" or "loop" or "service";

	/// <summary>
	/// Builds a validated <see cref="AppSettings"/> instance from layered configuration and arguments.
	/// </summary>
	/// <param name="configuration">The application configuration root.</param>
	/// <param name="environmentName">The host environment name.</param>
	/// <param name="args">Startup arguments.</param>
	/// <param name="settings">The parsed settings instance when parsing succeeds.</param>
	/// <param name="errorMessage">The validation error message when parsing fails.</param>
	/// <returns><c>true</c> when parsing succeeds; otherwise, <c>false</c>.</returns>
	public static bool TryCreate(IConfiguration configuration, string environmentName, string[] args, out AppSettings? settings, out string? errorMessage)
	{
		string? appName = configuration["APP_NAME"] ?? configuration["App:Name"];
		if (string.IsNullOrWhiteSpace(appName))
		{
			appName = "My First Container App";
		}

		string appEnvironment = configuration["APP_ENVIRONMENT"]
			?? configuration["App:Environment"]
			?? environmentName;
		string? appMessage = configuration["APP_MESSAGE"] ?? configuration["App:Message"];
		string? configuredRunMode = configuration["APP_MODE"] ?? configuration["App:RunMode"];
		string? requestedMode = args.Length > 0 ? args[0] : configuredRunMode;
		string runModeSource = args.Length > 0 ? "args" : string.IsNullOrWhiteSpace(configuredRunMode) ? "default" : "configuration";

		if (!TryParseRunMode(requestedMode, out string runMode, out errorMessage))
		{
			settings = null;
			return false;
		}

		string? heartbeatSecondsText = configuration["HEARTBEAT_SECONDS"] ?? configuration["App:HeartbeatSeconds"];
		int heartbeatSeconds = 5;
		if (!string.IsNullOrWhiteSpace(heartbeatSecondsText))
		{
			if (!int.TryParse(heartbeatSecondsText, out heartbeatSeconds) || heartbeatSeconds <= 0)
			{
				settings = null;
				errorMessage = "HEARTBEAT_SECONDS 必須是大於 0 的整數。";
				return false;
			}
		}

		settings = new AppSettings(appName, appEnvironment, appMessage, runMode, runModeSource, heartbeatSeconds);
		errorMessage = null;
		return true;
	}

	private static bool TryParseRunMode(string? requestedMode, out string runMode, out string? errorMessage)
	{
		if (string.IsNullOrWhiteSpace(requestedMode))
		{
			runMode = "once";
			errorMessage = null;
			return true;
		}

		runMode = requestedMode.Trim().ToLowerInvariant();
		if (runMode is "once" or "watch" or "loop" or "service")
		{
			errorMessage = null;
			return true;
		}

		errorMessage = $"無效的執行模式: '{requestedMode}'。允許值: once, watch, loop, service。";
		return false;
	}
}
