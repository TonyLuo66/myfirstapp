using System.Net;
using MyFirstApp.Business.Models.Dtos;

namespace MyFirstApp.Infrastructure.Presentation;

/// <summary>
/// Builds the HTML home page content.
/// </summary>
public static class HomePageRenderer
{
    /// <summary>
    /// Renders the home page HTML for the current service state.
    /// </summary>
    /// <param name="status">The system status payload.</param>
    /// <returns>The full HTML document string.</returns>
    public static string Render(SystemStatusDto status)
    {
        string heartbeatText = status.LastHeartbeatAt.HasValue
            ? status.LastHeartbeatAt.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
            : "尚未產生 heartbeat";

        string appMessage = string.IsNullOrWhiteSpace(status.AppMessage)
            ? "未設定 APP_MESSAGE"
            : WebUtility.HtmlEncode(status.AppMessage);

        return $$"""
<!DOCTYPE html>
<html lang="zh-Hant">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>{{WebUtility.HtmlEncode(status.AppName)}}</title>
    <style>
        :root {
            color-scheme: light;
            --bg: linear-gradient(135deg, #f5f1e8 0%, #dce7f2 100%);
            --card: rgba(255, 255, 255, 0.85);
            --text: #16202a;
            --muted: #506172;
            --accent: #0f766e;
            --accent-soft: #d6f3ef;
            --border: rgba(15, 118, 110, 0.16);
            --shadow: 0 20px 45px rgba(22, 32, 42, 0.12);
        }

        * {
            box-sizing: border-box;
        }

        body {
            margin: 0;
            min-height: 100vh;
            font-family: "Segoe UI", "Noto Sans TC", sans-serif;
            background: var(--bg);
            color: var(--text);
            display: grid;
            place-items: center;
            padding: 24px;
        }

        .panel {
            width: min(760px, 100%);
            background: var(--card);
            border: 1px solid var(--border);
            box-shadow: var(--shadow);
            backdrop-filter: blur(14px);
            border-radius: 28px;
            overflow: hidden;
        }

        .hero {
            padding: 32px 32px 20px;
            border-bottom: 1px solid rgba(15, 118, 110, 0.12);
        }

        .badge {
            display: inline-flex;
            padding: 6px 12px;
            border-radius: 999px;
            background: var(--accent-soft);
            color: var(--accent);
            font-size: 12px;
            font-weight: 700;
            letter-spacing: 0.08em;
            text-transform: uppercase;
        }

        h1 {
            margin: 14px 0 8px;
            font-size: clamp(2rem, 4vw, 3.5rem);
            line-height: 1;
        }

        p {
            margin: 0;
            color: var(--muted);
            line-height: 1.7;
        }

        .grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
            gap: 14px;
            padding: 24px 32px 32px;
        }

        .card {
            background: rgba(255, 255, 255, 0.75);
            border-radius: 20px;
            padding: 18px;
            border: 1px solid rgba(15, 118, 110, 0.1);
        }

        .label {
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: 0.08em;
            color: var(--muted);
            margin-bottom: 10px;
        }

        .value {
            font-size: 1.05rem;
            font-weight: 700;
            word-break: break-word;
        }

        .footer {
            padding: 0 32px 32px;
            font-size: 14px;
            color: var(--muted);
        }

        a {
            color: var(--accent);
            text-decoration: none;
        }
    </style>
</head>
<body>
    <main class="panel">
        <section class="hero">
            <span class="badge">service online</span>
            <h1>{{WebUtility.HtmlEncode(status.AppName)}}</h1>
            <p>{{appMessage}}</p>
        </section>
        <section class="grid">
            <article class="card">
                <div class="label">Environment</div>
                <div class="value">{{WebUtility.HtmlEncode(status.AppEnvironment)}}</div>
            </article>
            <article class="card">
                <div class="label">Run Mode</div>
                <div class="value">{{WebUtility.HtmlEncode(status.RunMode)}}</div>
            </article>
            <article class="card">
                <div class="label">Heartbeats</div>
                <div class="value">{{status.HeartbeatCount}}</div>
            </article>
            <article class="card">
                <div class="label">Last Heartbeat</div>
                <div class="value">{{WebUtility.HtmlEncode(heartbeatText)}}</div>
            </article>
        </section>
        <div class="footer">
            API: <a href="/swagger">/swagger</a> | <a href="/api/system-status">/api/system-status</a> | <a href="/api/system-status/health">/api/system-status/health</a> | <a href="/api/application-profiles">/api/application-profiles</a>
        </div>
    </main>
</body>
</html>
""";
    }
}