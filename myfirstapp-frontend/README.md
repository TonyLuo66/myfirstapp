# myfirstapp-frontend

這是對應 myfirstapp 後端 API 的前端專案，採用 Vue 3 + TypeScript + Vite。

## 功能

- 系統狀態儀表板
- Application Profiles 列表 / 詳細 / 新增 / 編輯
- 透過統一 API client 串接後端 `rtnCode/rtnMsg/data` 回傳格式
- 使用環境變數切換 API Base URL

## 開發方式

1. 先啟動後端 API：`dotnet run --launch-profile myfirstapp-http`
2. 進入前端資料夾：`cd myfirstapp-frontend`
3. 安裝套件：`npm install`
4. 啟動前端：`npm run dev`

預設 Vite 開在 `http://localhost:5173`。

## API 銜接規則

- 本機開發：Vite 會將 `/api` 與 `/health` 代理到 `http://localhost:5070`
- 若前後端分開部署，可在 `.env` 中設定 `VITE_API_BASE_URL`

## 結構

- `src/services/`：API 存取層
- `src/types/`：前端 DTO / API envelope 型別
- `src/views/`：頁面
- `src/components/`：可重用 UI 元件