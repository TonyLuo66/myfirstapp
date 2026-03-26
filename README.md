# myfirstapp

這是一個以 .NET 10 撰寫的容器化 Web API 範例，已整理成分層架構，並補上 Dapper、Repository、SQL Server、appsettings 分層設定、Secret 設計與 launchSettings，適合拿來練習 Docker 打包、HTTP 健康檢查、環境變數設定與對外部署。

## 文件導覽

- Render 部署說明: docs/RENDER_DEPLOYMENT.md
- 正式交接發布說明: docs/RELEASE_HANDOVER.md

## 功能概覽

- 提供首頁、Swagger、健康檢查與 JSON 狀態頁面
- 提供以 Dapper + Query/Command Repository 查詢 SQL Server 的範例 API
- 可透過環境變數或命令列參數控制 heartbeat 是否啟用
- API 採用統一回傳格式: rtnCode、rtnMsg、data
- 狀態查詢已拆成 Controller、Service、DTO、Validator
- 已加入請求摘要日誌與全域例外處理 middleware
- 適合部署到需要 HTTP 端點的雲端平台

## 支援的設定

| 名稱 | 用途 | 預設值 |
| --- | --- | --- |
| APP_NAME | 顯示在啟動訊息中的應用名稱 | My First Container App |
| APP_ENVIRONMENT | 環境名稱 | local |
| APP_MESSAGE | 額外顯示的自訂訊息 | 空白 |
| APP_MODE | 執行模式，允許 once、watch、loop、service | once |
| HEARTBEAT_SECONDS | heartbeat 啟用時的秒數間隔，必須大於 0 | 5 |
| DB_CONNECTION_STRING | 覆寫應用資料庫連線字串 | Server=(localdb)\MSSQLLocalDB;Database=myfirstapp;... |
| DB_MASTER_CONNECTION_STRING | 覆寫 master 資料庫連線字串 | Server=(localdb)\MSSQLLocalDB;Database=master;... |
| DATABASE_SEED_ON_STARTUP | 啟動時是否建立示範資料 | true |
| DATABASE_INITIALIZE_ON_STARTUP | 啟動時是否執行資料庫初始化流程 | true |
| DATABASE_CREATE_IF_MISSING | 啟動時是否透過 master 連線執行 CREATE DATABASE | true |

## 設定來源順序

- launchSettings: 本機開發啟動 profile
- appsettings.json: 基本預設值
- appsettings.Development.json / appsettings.Production.json: 環境分層設定
- appsettings.Secrets.json: 本機不入版的敏感設定
- dotnet user-secrets: 開發機敏感設定
- 環境變數: 最後覆蓋，例如 APP_MODE、DB_CONNECTION_STRING
- 命令列參數: 目前用來覆蓋 run mode

## 執行模式

- once: 啟動 HTTP 服務，但不啟動背景 heartbeat
- watch: 啟動 HTTP 服務，並定期輸出 heartbeat
- loop: 啟動 HTTP 服務，並定期輸出 heartbeat
- service: 啟動 HTTP 服務，並定期輸出 heartbeat

如果 APP_MODE 或命令列參數給了不支援的值，程式會直接結束並回傳非 0 exit code。

## 本機執行

### 已驗證的本機環境

以下流程已在 Windows + SQL Server Express + SSMS 上實際驗證成功：

- SQL Server 執行個體：`.\SQLEXPRESS`
- 驗證模式：Windows 驗證
- 管理工具：SSMS 22
- 專案啟動方式：`dotnet run --launch-profile myfirstapp-http`

```powershell
dotnet run --launch-profile myfirstapp-http
```

開啟瀏覽器驗證：

- http://localhost:5070/
- http://localhost:5070/swagger
- http://localhost:5070/health
- http://localhost:5070/api/status
- http://localhost:5070/api/system-status
- http://localhost:5070/api/system-status/health
- http://localhost:5070/api/application-profiles
- http://localhost:5070/api/application-profiles/ops-console

啟用 heartbeat 範例：

```powershell
$env:APP_MODE="service"
$env:HEARTBEAT_SECONDS="5"
dotnet run
```

若你想用不啟用 heartbeat 的本機 profile：

```powershell
dotnet run --launch-profile myfirstapp-safe
```

### 本機 SQL Server Express 設定步驟

若你跟目前這次驗證一樣使用 SQL Server Express，本機建議直接用 `user-secrets`：

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.\SQLEXPRESS;Database=myfirstapp;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
dotnet user-secrets set "ConnectionStrings:MasterConnection" "Server=.\SQLEXPRESS;Database=master;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
dotnet user-secrets set "Database:SeedOnStartup" "true"
dotnet user-secrets set "Database:InitializeOnStartup" "true"
dotnet user-secrets set "Database:CreateDatabaseIfMissing" "true"
```

設定完成後啟動專案：

```powershell
dotnet run --launch-profile myfirstapp-http
```

程式會在啟動時自動：

- 連線到 `master`
- 建立 `myfirstapp` 資料庫
- 建立 `dbo.app_profiles` 資料表
- 寫入範例資料

### 使用 SSMS 驗證

在 SSMS 連線時，使用：

- 伺服器名稱：`.\SQLEXPRESS`
- 驗證：`Windows 驗證`

連線成功後，可用下列 SQL 驗證資料表內容：

```sql
USE myfirstapp;

SELECT
	profile_key,
	display_name,
	owner_team,
	environment,
	is_active,
	updated_at
FROM dbo.app_profiles
ORDER BY profile_key;
```

這次實測時可看到以下資料：

- `legacy-batch`
- `member-portal`
- `ops-console`
- `crm-api`

## Dapper 查詢範例

搜尋資料：

```powershell
dotnet run --launch-profile myfirstapp-http
```

```text
GET /api/application-profiles
GET /api/application-profiles?keyword=portal&pageNumber=1&pageSize=10
GET /api/application-profiles/ops-console
POST /api/application-profiles
PUT /api/application-profiles/ops-console
```

這組 API 會在啟動時自動連到 SQL Server，建立 `myfirstapp` 資料庫與 `dbo.app_profiles` 資料表，並寫入示範資料。

新增範例：

```json
{
	"profileKey": "crm-api",
	"displayName": "CRM API",
	"ownerTeam": "CRM Team",
	"environment": "UAT",
	"isActive": true
}
```

更新範例：

```json
{
	"displayName": "CRM API v2",
	"ownerTeam": "CRM Team",
	"environment": "Production",
	"isActive": true
}
```

### 已驗證的測試流程

以下流程已實際測通：

1. `GET /api/application-profiles`
2. `POST /api/application-profiles`
3. `PUT /api/application-profiles/crm-api`
4. 重新用 SSMS 查 `dbo.app_profiles` 確認資料已更新
5. 重複新增相同 `profileKey`，回傳 `409` 與 `rtnCode = 1409`
6. 查詢不存在的 `profileKey`，回傳 `404` 與 `rtnCode = 1404`

PowerShell 測試範例：

```powershell
$createBody = @{
	profileKey = "crm-api"
	displayName = "CRM API"
	ownerTeam = "CRM Team"
	environment = "UAT"
	isActive = $true
} | ConvertTo-Json

Invoke-RestMethod -Method Post -Uri "http://localhost:5070/api/application-profiles" -ContentType "application/json" -Body $createBody
```

```powershell
$updateBody = @{
	displayName = "CRM API v2"
	ownerTeam = "CRM Team"
	environment = "Production"
	isActive = $true
} | ConvertTo-Json

Invoke-RestMethod -Method Put -Uri "http://localhost:5070/api/application-profiles/crm-api" -ContentType "application/json" -Body $updateBody
```

## Secret 設計

建議不要把正式資料庫密碼直接寫進 appsettings.json。

本機開發可使用以下其中一種：

- 複製 appsettings.Secrets.example.json 為 appsettings.Secrets.json，填入實際 SQL Server 連線資訊
- 使用 dotnet user-secrets

user-secrets 範例：

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=tcp:your-sql-host,1433;Database=myfirstapp;User ID=app_user;Password=***;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True"
dotnet user-secrets set "ConnectionStrings:MasterConnection" "Server=tcp:your-sql-host,1433;Database=master;User ID=app_admin;Password=***;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True"
dotnet user-secrets set "Database:InitializeOnStartup" "true"
dotnet user-secrets set "Database:CreateDatabaseIfMissing" "false"
```

若你要照本機 SQL Express 版本重做，請改用：

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.\SQLEXPRESS;Database=myfirstapp;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
dotnet user-secrets set "ConnectionStrings:MasterConnection" "Server=.\SQLEXPRESS;Database=master;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
dotnet user-secrets set "Database:SeedOnStartup" "true"
dotnet user-secrets set "Database:InitializeOnStartup" "true"
dotnet user-secrets set "Database:CreateDatabaseIfMissing" "true"
```

部署環境建議改用：

- 平台 Secret / Environment Variable
- Key Vault 類型的外部 Secret 管理服務

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

如果只是驗證容器是否能啟動，不想在 Docker smoke test 連 SQL Server，可以暫時關閉初始化：

```powershell
docker run --rm -p 8080:8080 `
  -e DATABASE_INITIALIZE_ON_STARTUP=false `
  -e DB_CONNECTION_STRING="Server=tcp:placeholder,1433;Database=myfirstapp;User ID=demo;Password=demo;Encrypt=True;TrustServerCertificate=False;" `
  myfirstapp:local
```

啟用 heartbeat：

```powershell
docker run --rm -p 8080:8080 -e APP_MODE=service -e APP_ENVIRONMENT=test -e HEARTBEAT_SECONDS=5 myfirstapp:local
```

如果部署帳號沒有 `CREATE DATABASE` 權限，但資料庫已由 DBA 建好，可改成：

```powershell
docker run --rm -p 8080:8080 `
  -e APP_MODE=service `
  -e DB_CONNECTION_STRING="Server=tcp:<app-sql-host>,1433;Database=myfirstapp;User ID=<app_user>;Password=<password>;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True" `
  -e DATABASE_CREATE_IF_MISSING=false `
  -e DATABASE_SEED_ON_STARTUP=false `
  myfirstapp:local
```

## 部署提醒

- 這個專案現在提供 HTTP 端點，適合部署為 Web Service
- 若你想同時展示背景工作效果，請設定 APP_MODE=service、watch 或 loop
- 建議優先用 /swagger 或 /api/system-status/health 驗證平台是否成功啟動服務
- /health 與 /api/status 目前保留為相容驗證路徑
- 狀態 API 統一包成 rtnCode、rtnMsg、data，部署驗證時請確認 data 內欄位是否符合預期
- 正式部署請改用平台提供的 SQL Server 或受管資料庫，不要依賴 LocalDB
- 啟動帳號若沒有 CREATE DATABASE 權限，請先由 DBA 建立資料庫，再把 SeedOnStartup 關閉

## SQL Server 部署檢查清單

部署到正式或測試環境前，建議逐項確認：

1. 應用程式使用的是平台提供的 SQL Server 連線字串，不是本機的 `.\\SQLEXPRESS` 或 `(localdb)\\MSSQLLocalDB`
2. 正式環境的 `ConnectionStrings:DefaultConnection` 與 `ConnectionStrings:MasterConnection` 都從平台 Secret 注入
3. 若部署帳號沒有 `CREATE DATABASE` 權限，將 `Database:SeedOnStartup` 設為 `false`
4. 若資料庫由 DBA 先建立，請確認已存在 `dbo.app_profiles` 資料表，或先執行建表腳本
5. 防火牆、VNet、白名單或雲端資料庫存取規則已允許目前部署平台連線
6. 正式環境不要使用 `TrustServerCertificate=True`，改用正確憑證設定
7. 部署完成後，先驗證 `/swagger`、`/api/system-status/health`、`/api/application-profiles`
8. 觀察平台日誌，確認沒有出現資料庫連線失敗、登入失敗、權限不足、建表失敗

### 部署環境建議變數

可依部署平台改成環境變數或 Secret：

Render Environment 可直接貼上的最終清單：

```text
ASPNETCORE_ENVIRONMENT=Production
APP_MODE=service
APP_MESSAGE=Production deployment
HEARTBEAT_SECONDS=10
DB_CONNECTION_STRING=Server=tcp:<your-sql-host>,1433;Database=myfirstapp;User ID=<app_user>;Password=<password>;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True
DB_MASTER_CONNECTION_STRING=Server=tcp:<your-sql-host>,1433;Database=master;User ID=<admin_user>;Password=<password>;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True
DATABASE_SEED_ON_STARTUP=false
DATABASE_INITIALIZE_ON_STARTUP=true
DATABASE_CREATE_IF_MISSING=false
```

如果正式環境的資料庫與資料表都已由 DBA 先建立，且部署帳號沒有 `master` 權限，維持上面這組即可，不需要改回 `DATABASE_CREATE_IF_MISSING=true`。

### 沒有 CREATE DATABASE 權限時的最小建表腳本

若資料庫已先建立，但應用程式帳號不能建 DB，可先由 DBA 或有權限帳號執行：

```sql
IF OBJECT_ID(N'dbo.app_profiles', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.app_profiles (
		profile_key NVARCHAR(40) NOT NULL PRIMARY KEY,
		display_name NVARCHAR(100) NOT NULL,
		owner_team NVARCHAR(100) NOT NULL,
		environment NVARCHAR(30) NOT NULL,
		is_active BIT NOT NULL,
		updated_at DATETIME2 NOT NULL
	);
END;
```

## 錯誤回應格式

當 API 發生未預期錯誤時，會回傳統一格式：

```json
{
	"rtnCode": "9999",
	"rtnMsg": "系統發生未預期錯誤。",
	"data": {
		"traceId": "...",
		"path": "/api/example",
		"occurredAt": "2026-03-26T00:00:00Z"
	}
}
```

一般驗證錯誤則使用 rtnCode = 1001。

## 錯誤碼規格表

| rtnCode | 說明 | 使用情境 |
| --- | --- | --- |
| 0000 | 成功 | 正常查詢成功 |
| 1001 | 驗證失敗 | 查詢物件缺失或格式不符 |
| 1002 | 無效查詢參數 | pageSize 超出範圍、keyword 太短、responseMode 無效 |
| 1404 | 查無資料 | 單筆查詢找不到對應 profileKey |
| 1409 | 資料已存在 | 新增重複 profileKey |
| 2001 | 資料庫錯誤 | SQL Server / Dapper 查詢失敗 |
| 9999 | 未預期系統錯誤 | 未捕捉例外 |
