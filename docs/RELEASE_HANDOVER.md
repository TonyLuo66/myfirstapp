# 正式發布交接說明

本文件提供給後續維護、部署或接手同仁，作為目前版本的正式交接摘要。

## 1. 專案資訊

- 專案名稱：myfirstapp
- GitHub 倉庫：https://github.com/TonyLuo66/myfirstapp.git
- 技術棧：.NET 10、ASP.NET Core Web API、Dapper、SQL Server、Docker
- 部署型態：Render Web Service

## 2. 本次版本重點

本次版本已從原本的簡單程式，整理為可部署的 Web API 架構，包含：

- Controller / Service / Validator / Repository 分層
- Swagger 文件
- 統一 API 回傳格式：rtnCode、rtnMsg、data
- 全域例外處理 middleware
- Request logging middleware
- Dapper + SQL Server 實作
- Application profile 查詢、新增、更新 API
- appsettings 分層設定與 secret 設計
- Docker 化部署能力

## 3. 主要 API

### 系統檢查

- GET /
- GET /swagger
- GET /health
- GET /api/status
- GET /api/system-status
- GET /api/system-status/health

### Application Profiles

- GET /api/application-profiles
- GET /api/application-profiles/{profileKey}
- POST /api/application-profiles
- PUT /api/application-profiles/{profileKey}

## 4. 重要設定來源

系統設定讀取順序如下：

1. launchSettings
2. appsettings.json
3. appsettings.{Environment}.json
4. appsettings.Secrets.json
5. environment variables
6. command arguments

正式環境請優先使用 environment variables 或平台 secret，不要提交 appsettings.Secrets.json。

## 5. 資料庫說明

目前資料表：

- dbo.app_profiles

主要欄位：

- profile_key
- display_name
- owner_team
- environment
- is_active
- updated_at

注意事項：

- 系統可在啟動時自動建 DB / 建表 / seed 資料
- 正式環境若無 CREATE DATABASE 權限，需先由 DBA 建庫建表
- 正式環境建議 DATABASE_SEED_ON_STARTUP=false
- 正式環境若只需要連既有 DB，建議 DATABASE_CREATE_IF_MISSING=false

## 6. 正式環境建議變數

```text
ASPNETCORE_ENVIRONMENT=Production
APP_MODE=service
APP_MESSAGE=Production deployment
HEARTBEAT_SECONDS=10
DB_CONNECTION_STRING=<application-db-connection-string>
DB_MASTER_CONNECTION_STRING=<master-db-connection-string>
DATABASE_SEED_ON_STARTUP=false
DATABASE_INITIALIZE_ON_STARTUP=true
DATABASE_CREATE_IF_MISSING=false
```

## 7. 本機驗證方式

已驗證流程：

- 使用 SQL Server Express `.\SQLEXPRESS`
- 使用 Windows Authentication
- 使用 SSMS 驗證 dbo.app_profiles
- 使用 `dotnet run --launch-profile myfirstapp-http` 啟動

## 8. 已知注意事項

1. 本機可用 SQL Server Express，但正式部署不可依賴本機資料庫
2. Render 只負責跑 Web Service，不提供 SQL Server，需外接資料庫
3. 舊的 SQLite 測試資料已不再使用，不應入 Git，也不應打進 Docker image
4. appsettings.Secrets.json 不可提交到 GitHub

## 9. GitHub 檔案檢查結果

目前最新 commit 已確認：

- 已包含：程式碼、Dockerfile、appsettings 範本、README、launchSettings
- 未包含：appsettings.Secrets.json、data 目錄、本機 SQLite 資料檔

這表示目前 GitHub 倉庫內容符合可公開版控的基本要求。

## 10. 建議交接流程

若交給下一位同仁，建議順序如下：

1. 先看 README
2. 再看 docs/RENDER_DEPLOYMENT.md
3. 先在本機用 SQL Server Express 跑起來
4. 再設定 Render 環境變數與外部 SQL Server
5. 部署後先驗證 /swagger 與 /api/application-profiles

## 11. 建議後續工作

- 補上 migration 或獨立 DB 初始化流程
- 補上整合測試
- 規劃正式環境的 Swagger 權限策略
- 整理更完整的 API 規格文件
