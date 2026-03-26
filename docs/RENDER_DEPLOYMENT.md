# Render 部署說明

本文件整理目前這個專案部署到 Render Web Service 時，實際需要填寫的設定與 SQL Server 注意事項。

## 1. 部署前確認

- GitHub 倉庫：已存在且可供 Render 連線
- 專案型態：Docker 化 ASP.NET Core Web API
- 容器 Port：8080
- 資料庫：外部 SQL Server，不是 Render 內建資料庫

Render 可以部署這個 Web API，但 SQL Server 需要另外準備，例如：

- Azure SQL Database
- 公司內部 SQL Server
- 其他可從公網或專線連線的 SQL Server

## 2. 在 Render 建立服務

建議選擇：

- New +
- Web Service
- 連接 GitHub 倉庫
- 選取 myfirstapp

若 Render 有偵測到 Dockerfile，直接使用 Docker 部署即可。

## 3. 建議的 Render 基本設定

- Name: myfirstapp
- Region: 選離資料庫最近的區域
- Branch: main
- Runtime: Docker
- Auto Deploy: Yes

## 4. Render 環境變數

請在 Render 的 Environment 區塊填入以下值。

必要變數：

```text
ASPNETCORE_ENVIRONMENT=Production
APP_MODE=service
APP_MESSAGE=Render production deployment
HEARTBEAT_SECONDS=10
DB_CONNECTION_STRING=Server=tcp:<your-sql-host>,1433;Database=myfirstapp;User ID=<app_user>;Password=<password>;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True
DB_MASTER_CONNECTION_STRING=Server=tcp:<your-sql-host>,1433;Database=master;User ID=<admin_user>;Password=<password>;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True
DATABASE_SEED_ON_STARTUP=false
DATABASE_INITIALIZE_ON_STARTUP=true
DATABASE_CREATE_IF_MISSING=false
```

說明：

- APP_MODE 建議用 service，保留 heartbeat 與背景服務行為
- HEARTBEAT_SECONDS 正式環境建議不要設太短，避免無意義日誌過多
- DB_CONNECTION_STRING 給應用程式平常查詢與寫入使用
- DB_MASTER_CONNECTION_STRING 只有在需要建庫時才會用到
- DATABASE_SEED_ON_STARTUP 在正式環境通常應設為 false
- DATABASE_INITIALIZE_ON_STARTUP 可在純 HTTP smoke test 時設為 false
- DATABASE_CREATE_IF_MISSING 若部署帳號無法連 `master` 或沒有 CREATE DATABASE 權限，應設為 false

## 5. SQL Server 設定建議

正式部署時，建議用以下原則：

1. 應用程式帳號只給必要權限，不要直接使用 sa 或高權限帳號
2. 如果資料庫與資料表已由 DBA 建好，將 DATABASE_SEED_ON_STARTUP 設為 false
3. 如果 DBA 已先建好資料庫，將 DATABASE_CREATE_IF_MISSING 設為 false
4. 不要在正式環境使用 TrustServerCertificate=True
5. 確認 SQL Server 防火牆或網路規則允許 Render 出站連線
6. 若使用公司內網 SQL Server，需先處理 VPN、Private Link 或白名單問題

## 6. 沒有建庫權限時怎麼做

如果 Render 上的應用程式帳號不能 CREATE DATABASE，請先由 DBA 建立資料庫與資料表。

最小建表腳本如下：

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

## 7. 部署後驗證順序

部署完成後，依序檢查：

1. 首頁 `/`
2. Swagger `/swagger`
3. 健康檢查 `/api/system-status/health`
4. 狀態查詢 `/api/system-status`
5. 資料查詢 `/api/application-profiles`

如果 `/api/application-profiles` 失敗，但 `/api/system-status/health` 正常，通常代表：

- DB 連線字串錯誤
- SQL Server 防火牆未開
- 登入帳號權限不足
- 資料表尚未建立

## 8. 常見錯誤排查

### 啟動成功但查 DB 失敗

優先檢查：

- DB_CONNECTION_STRING
- SQL Server 是否允許來自 Render 的連線
- 資料表是否存在

### 啟動直接失敗

優先檢查：

- DB_MASTER_CONNECTION_STRING 是否缺少或錯誤
- APP_MODE 是否填了不支援的值
- Render logs 中是否出現設定解析錯誤

### API 回傳 500 與 rtnCode=2001

代表資料庫存取失敗，通常是：

- 連線失敗
- 權限不足
- SQL 物件不存在

## 9. 建議的正式做法

若是示範環境：

- 可保留 APP_MESSAGE 做版本說明
- 可保留 Swagger 方便驗證

若是正式環境：

- 建議限制 Swagger 對外公開策略
- 將 secrets 全部改由 Render Secret 管理
- 建議把資料庫初始化與 migration 流程獨立，不依賴應用程式啟動自動建置
