if (!AppSettings.TryCreate(args, out var settings, out var errorMessage))
{
	Console.Error.WriteLine(errorMessage);
	return;
}

var appSettings = settings!;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(appSettings);
builder.Services.AddSingleton<AppRuntimeState>();
builder.Services.AddHostedService<HeartbeatWorker>();

var app = builder.Build();

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
Console.WriteLine("HTTP endpoints: /, /health, /api/status");

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

app.MapGet("/health", (AppRuntimeState runtimeState) =>
{
 	var snapshot = runtimeState.CreateSnapshot();
	return Results.Ok(new
	{
		status = "ok",
		appName = appSettings.AppName,
		environment = appSettings.AppEnvironment,
		runMode = appSettings.RunMode,
		heartbeatCount = snapshot.HeartbeatCount,
		lastHeartbeatAtUtc = snapshot.LastHeartbeatAtUtc,
		startedAtUtc
	});
});

app.MapGet("/api/status", (HttpContext httpContext, AppRuntimeState runtimeState) =>
{
	var snapshot = runtimeState.CreateSnapshot();
	return Results.Ok(new
	{
		appName = appSettings.AppName,
		appEnvironment = appSettings.AppEnvironment,
		appMessage = appSettings.AppMessage,
		runMode = appSettings.RunMode,
		runModeSource = appSettings.RunModeSource,
		heartbeatSeconds = appSettings.HeartbeatSeconds,
		heartbeatCount = snapshot.HeartbeatCount,
		lastHeartbeatAtUtc = snapshot.LastHeartbeatAtUtc,
		startedAtUtc,
		machineName = Environment.MachineName,
		currentDirectory = Directory.GetCurrentDirectory(),
		dotnetVersion = Environment.Version.ToString(),
		osVersion = Environment.OSVersion.ToString(),
		requestHost = httpContext.Request.Host.Value,
		requestScheme = httpContext.Request.Scheme
	});
});

app.MapGet("/", (HttpContext httpContext, AppRuntimeState runtimeState) =>
{
	var snapshot = runtimeState.CreateSnapshot();
	var messageHtml = string.IsNullOrWhiteSpace(appSettings.AppMessage)
		? ""
		: $"<p class=\"message\">{System.Net.WebUtility.HtmlEncode(appSettings.AppMessage)}</p>";
	var lastHeartbeatText = snapshot.LastHeartbeatAtUtc?.ToString("yyyy-MM-dd HH:mm:ss zzz") ?? "Not started";

	var html = $$"""
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>{{System.Net.WebUtility.HtmlEncode(appSettings.AppName)}}</title>
  <style>
    :root {
      color-scheme: light;
      --bg: #f3efe7;
      --panel: rgba(255,255,255,0.88);
      --text: #1e1c1a;
      --muted: #5e5a55;
      --accent: #c85a3d;
      --accent-dark: #8f3e2b;
      --border: rgba(30, 28, 26, 0.1);
    }
    * { box-sizing: border-box; }
    body {
      margin: 0;
      font-family: "Segoe UI", "Noto Sans TC", sans-serif;
      color: var(--text);
      background:
        radial-gradient(circle at top left, rgba(200,90,61,0.16), transparent 32%),
        linear-gradient(135deg, #f8f4ec 0%, var(--bg) 55%, #ebe3d5 100%);
      min-height: 100vh;
    }
    .shell {
      max-width: 960px;
      margin: 0 auto;
      padding: 48px 20px 64px;
    }
    .hero {
      background: var(--panel);
      border: 1px solid var(--border);
      border-radius: 28px;
      padding: 28px;
      box-shadow: 0 24px 80px rgba(56, 34, 21, 0.08);
      backdrop-filter: blur(10px);
    }
    .eyebrow {
      display: inline-block;
      margin-bottom: 12px;
      font-size: 13px;
      letter-spacing: 0.12em;
      text-transform: uppercase;
      color: var(--accent-dark);
    }
    h1 { margin: 0 0 12px; font-size: clamp(32px, 6vw, 56px); line-height: 1; }
    p { margin: 0; color: var(--muted); font-size: 16px; line-height: 1.6; }
    .message { margin-top: 16px; color: var(--accent-dark); font-weight: 600; }
    .grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
      gap: 16px;
      margin-top: 24px;
    }
    .card {
      padding: 18px;
      border-radius: 20px;
      background: rgba(255,255,255,0.82);
      border: 1px solid var(--border);
    }
    .label { font-size: 12px; text-transform: uppercase; letter-spacing: 0.1em; color: var(--muted); }
    .value { margin-top: 8px; font-size: 22px; font-weight: 700; color: var(--text); word-break: break-word; }
    .links { display: flex; flex-wrap: wrap; gap: 12px; margin-top: 24px; }
    a { color: white; text-decoration: none; background: var(--accent); padding: 12px 16px; border-radius: 999px; font-weight: 600; }
    a.secondary { background: transparent; color: var(--accent-dark); border: 1px solid rgba(143,62,43,0.25); }
  </style>
</head>
<body>
  <main class="shell">
    <section class="hero">
      <span class="eyebrow">HTTP Demo Ready</span>
      <h1>{{System.Net.WebUtility.HtmlEncode(appSettings.AppName)}}</h1>
      <p>這個服務已經改成可透過網址驗證的最小 Web App。你可以用這個首頁、健康檢查與 JSON 狀態頁面確認部署是否成功。</p>
      {{messageHtml}}
      <div class="grid">
        <article class="card"><div class="label">Environment</div><div class="value">{{System.Net.WebUtility.HtmlEncode(appSettings.AppEnvironment)}}</div></article>
        <article class="card"><div class="label">Run Mode</div><div class="value">{{System.Net.WebUtility.HtmlEncode(appSettings.RunMode)}}</div></article>
        <article class="card"><div class="label">Heartbeat Count</div><div class="value">{{snapshot.HeartbeatCount}}</div></article>
        <article class="card"><div class="label">Last Heartbeat (UTC)</div><div class="value">{{lastHeartbeatText}}</div></article>
        <article class="card"><div class="label">Started At (UTC)</div><div class="value">{{startedAtUtc:yyyy-MM-dd HH:mm:ss zzz}}</div></article>
        <article class="card"><div class="label">Current Host</div><div class="value">{{System.Net.WebUtility.HtmlEncode(httpContext.Request.Host.Value)}}</div></article>
      </div>
      <div class="links">
        <a href="/health">Open /health</a>
        <a class="secondary" href="/api/status">Open /api/status</a>
      </div>
    </section>
  </main>
</body>
</html>
""";

	return Results.Content(html, "text/html; charset=utf-8");
});

await app.RunAsync();