using System.Runtime.InteropServices;
using System.Runtime.Loader;

if (!AppSettings.TryCreate(args, out var settings, out var errorMessage))
{
	Console.Error.WriteLine(errorMessage);
	return 1;
}

var appSettings = settings!;

var startedAtUtc = DateTimeOffset.UtcNow;

Console.WriteLine(appSettings.AppName);
Console.WriteLine(new string('=', appSettings.AppName.Length));
Console.WriteLine($"Started at (UTC): {startedAtUtc:yyyy-MM-dd HH:mm:ss zzz}");
Console.WriteLine($"Machine name: {Environment.MachineName}");
Console.WriteLine($"Current directory: {Directory.GetCurrentDirectory()}");
Console.WriteLine($".NET version: {Environment.Version}");
Console.WriteLine($"OS version: {Environment.OSVersion}");
Console.WriteLine($"App environment: {appSettings.AppEnvironment}");
Console.WriteLine($"Run mode: {appSettings.RunMode}");
Console.WriteLine($"Run mode source: {appSettings.RunModeSource}");
Console.WriteLine($"Heartbeat seconds: {appSettings.HeartbeatSeconds}");
Console.WriteLine($"Argument count: {args.Length}");

if (args.Length > 0)
{
	Console.WriteLine("Arguments:");
	foreach (var arg in args)
	{
		Console.WriteLine($"- {arg}");
	}
}
else
{
	Console.WriteLine("No arguments were provided.");
}

if (!string.IsNullOrWhiteSpace(appSettings.AppMessage))
{
	Console.WriteLine($"Custom message: {appSettings.AppMessage}");
}

if (appSettings.IsLongRunning)
{
	Console.WriteLine($"Long-running mode enabled. Heartbeat interval: {appSettings.HeartbeatSeconds} second(s). Press Ctrl+C to stop the process.");

	using var shutdownTokenSource = new CancellationTokenSource();
	var shutdownRequested = false;
	string? shutdownSource = null;

	void RequestShutdown(string source)
	{
		if (shutdownRequested)
		{
			return;
		}

		shutdownRequested = true;
		shutdownSource = source;
		Console.WriteLine($"Shutdown signal received from {source}.");
		shutdownTokenSource.Cancel();
	}

	void OnCancelKeyPress(object? _, ConsoleCancelEventArgs eventArgs)
	{
		eventArgs.Cancel = true;
		RequestShutdown("Ctrl+C");
	}

	Console.CancelKeyPress += OnCancelKeyPress;

	void OnUnloading(AssemblyLoadContext _) => RequestShutdown("runtime unload");

	AssemblyLoadContext.Default.Unloading += OnUnloading;

	PosixSignalRegistration? sigTermRegistration = null;
	PosixSignalRegistration? sigIntRegistration = null;

	if (!OperatingSystem.IsWindows())
	{
		sigTermRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, signalContext =>
		{
			signalContext.Cancel = true;
			RequestShutdown("SIGTERM");
		});

		sigIntRegistration = PosixSignalRegistration.Create(PosixSignal.SIGINT, signalContext =>
		{
			signalContext.Cancel = true;
			RequestShutdown("SIGINT");
		});
	}

	var tick = 1;

	try
	{
		while (!shutdownTokenSource.Token.IsCancellationRequested)
		{
			Console.WriteLine($"Heartbeat {tick}: {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss zzz}");
			tick++;
			await Task.Delay(TimeSpan.FromSeconds(appSettings.HeartbeatSeconds), shutdownTokenSource.Token);
		}
	}
	catch (OperationCanceledException)
	{
	}
	finally
	{
		Console.CancelKeyPress -= OnCancelKeyPress;
		AssemblyLoadContext.Default.Unloading -= OnUnloading;
		sigTermRegistration?.Dispose();
		sigIntRegistration?.Dispose();
	}

	Console.WriteLine($"Shutdown source: {shutdownSource ?? "unknown"}");
	Console.WriteLine("Stopping service...");
	await Task.Delay(TimeSpan.FromSeconds(1));
	Console.WriteLine("Cleanup completed. Goodbye.");
}
else
{
	Console.WriteLine("Single-run mode completed.");
}

return 0;