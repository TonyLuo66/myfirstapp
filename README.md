# myfirstapp

這是一個以 .NET 10 撰寫的容器化主控台程式，適合拿來練習 Docker 打包、環境變數設定與背景服務部署。

## 功能概覽

- 預設以單次模式執行
- 可透過環境變數或命令列參數切換成長駐模式
- 支援容器停止時的優雅關閉
- 啟動時會輸出目前環境與執行資訊

## 支援的設定

| 名稱 | 用途 | 預設值 |
| --- | --- | --- |
| APP_NAME | 顯示在啟動訊息中的應用名稱 | My First Container App |
| APP_ENVIRONMENT | 環境名稱 | local |
| APP_MESSAGE | 額外顯示的自訂訊息 | 空白 |
| APP_MODE | 執行模式，允許 once、watch、loop、service | once |
| HEARTBEAT_SECONDS | 長駐模式下的心跳秒數，必須大於 0 | 5 |

## 執行模式

- once: 執行一次後結束
- watch: 長駐執行並定期輸出 heartbeat
- loop: 長駐執行並定期輸出 heartbeat
- service: 長駐執行並定期輸出 heartbeat

如果 APP_MODE 或命令列參數給了不支援的值，程式會直接結束並回傳非 0 exit code。

## 本機執行

```powershell
dotnet run
```

長駐模式範例：

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

長駐模式：

```powershell
docker run --rm -e APP_MODE=service -e APP_ENVIRONMENT=test -e HEARTBEAT_SECONDS=5 myfirstapp:local
```

## 部署提醒

- 這個專案目前不是 Web API，也沒有對外 HTTP port
- 若部署平台期待容器持續存活，請務必設定 APP_MODE=service、watch 或 loop
- 建議先在本機確認 log 與關閉行為，再推到 GitHub 或外部環境