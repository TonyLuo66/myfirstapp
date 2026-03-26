# 1. 使用微軟官方的 .NET SDK 作為建置環境
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# 2. 複製專案設定檔並還原相依套件
COPY *.csproj ./
RUN dotnet restore myfirstapp.csproj

# 3. 複製所有程式碼，並編譯成正式發行版 (Release)
COPY . ./
RUN dotnet publish myfirstapp.csproj -c Release -o out --no-restore

# 4. 為了讓 Image 體積變小，最終階段只使用 Runtime (執行環境)
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app/out .

# 5. 設定容器啟動時，要執行的指令
ENTRYPOINT ["dotnet", "myfirstapp.dll"]