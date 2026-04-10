# 1. 先建置 Vue 前端，產出可部署的靜態檔
FROM node:20-alpine AS frontend-build
WORKDIR /frontend
COPY myfirstapp-frontend/package*.json ./
RUN npm ci
COPY myfirstapp-frontend/ ./
RUN npm run build

# 2. 使用微軟官方的 .NET SDK 作為建置環境
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# 3. 複製專案設定檔並還原相依套件
COPY *.csproj ./
RUN dotnet restore myfirstapp.csproj

# 4. 複製所有程式碼，並編譯成正式發行版 (Release)
COPY . ./
RUN dotnet publish myfirstapp.csproj -c Release -o out --no-restore

# 5. Web App 需要 ASP.NET Core Runtime 才能提供 HTTP 服務
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
RUN apt-get update \
	&& apt-get install -y --no-install-recommends curl \
	&& rm -rf /var/lib/apt/lists/*
ENV ASPNETCORE_URLS=http://+:8080
ENV TZ=Asia/Taipei
EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 CMD curl --fail http://127.0.0.1:8080/api/system-status/health || exit 1
COPY --from=build /app/out .
COPY --from=frontend-build /frontend/dist ./wwwroot

# 6. 設定容器啟動時，要執行的指令
ENTRYPOINT ["dotnet", "myfirstapp.dll"]