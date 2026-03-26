internal sealed class AppSettings
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

	public string AppName { get; }

	public string AppEnvironment { get; }

	public string? AppMessage { get; }

	public string RunMode { get; }

	public string RunModeSource { get; }

	public int HeartbeatSeconds { get; }

	public bool IsLongRunning => RunMode is "watch" or "loop" or "service";

	public static bool TryCreate(string[] args, out AppSettings? settings, out string? errorMessage)
	{
		var appName = Environment.GetEnvironmentVariable("APP_NAME");
		if (string.IsNullOrWhiteSpace(appName))
		{
			appName = "My First Container App";
		}

		var appEnvironment = Environment.GetEnvironmentVariable("APP_ENVIRONMENT") ?? "local";
		var appMessage = Environment.GetEnvironmentVariable("APP_MESSAGE");
		var configuredRunMode = Environment.GetEnvironmentVariable("APP_MODE");
		var requestedMode = args.Length > 0 ? args[0] : configuredRunMode;
		var runModeSource = args.Length > 0 ? "args" : string.IsNullOrWhiteSpace(configuredRunMode) ? "default" : "APP_MODE";

		if (!TryParseRunMode(requestedMode, out var runMode, out errorMessage))
		{
			settings = null;
			return false;
		}

		var heartbeatSecondsText = Environment.GetEnvironmentVariable("HEARTBEAT_SECONDS");
		var heartbeatSeconds = 5;

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
