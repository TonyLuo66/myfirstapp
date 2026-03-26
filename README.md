# myfirstapp

這是一個以 .NET 10 撰寫的容器化最小 Web App，適合拿來練習 Docker 打包、HTTP 健康檢查、環境變數設定與對外部署。

## 功能概覽

- 提供首頁、健康檢查與 JSON 狀態頁面
- 可透過環境變數或命令列參數控制 heartbeat 是否啟用
- 啟動時會輸出目前環境與執行資訊
- 適合部署到需要 HTTP 端點的雲端平台

## 支援的設定

| 名稱 | 用途 | 預設值 |
| --- | --- | --- |
| APP_NAME | 顯示在啟動訊息中的應用名稱 | My First Container App |
| APP_ENVIRONMENT | 環境名稱 | local |
| APP_MESSAGE | 額外顯示的自訂訊息 | 空白 |
| APP_MODE | 執行模式，允許 once、watch、loop、service | once |
| HEARTBEAT_SECONDS | heartbeat 啟用時的秒數間隔，必須大於 0 | 5 |

## 執行模式

- once: 啟動 HTTP 服務，但不啟動背景 heartbeat
- watch: 啟動 HTTP 服務，並定期輸出 heartbeat
- loop: 啟動 HTTP 服務，並定期輸出 heartbeat
- service: 啟動 HTTP 服務，並定期輸出 heartbeat

如果 APP_MODE 或命令列參數給了不支援的值，程式會直接結束並回傳非 0 exit code。

## 本機執行

```powershell
dotnet run
```

開啟瀏覽器驗證：

- http://localhost:5000/
- http://localhost:5000/health
- http://localhost:5000/api/status

啟用 heartbeat 範例：

```powershell
$env:APP_MODE="service"
$env:HEARTBEAT_SECONDS="5"
dotnet run
```

## Docker 建置

```powershell
docker build -t myfirstapp:local .
```

## Docker 執行

單次模式：

```powershell
docker run --rm myfirstapp:local
```

對外開放 HTTP port：

```powershell
docker run --rm -p 8080:8080 myfirstapp:local
```

啟用 heartbeat：

```powershell
docker run --rm -p 8080:8080 -e APP_MODE=service -e APP_ENVIRONMENT=test -e HEARTBEAT_SECONDS=5 myfirstapp:local
```

## 部署提醒

- 這個專案現在提供 HTTP 端點，適合部署為 Web Service
- 若你想同時展示背景工作效果，請設定 APP_MODE=service、watch 或 loop
- 建議優先用 /health 驗證平台是否成功啟動服務
- 建議用 /api/status 檢查環境變數與 heartbeat 狀態是否符合預期