# 使用 .NET 8 SDK 构建
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 复制 csproj 并还原依赖
COPY *.sln ./
COPY aspnetapp/*.csproj ./aspnetapp/
RUN dotnet restore ./aspnetapp/aspnetapp.csproj

# 复制源代码并编译发布
COPY aspnetapp/. ./aspnetapp/
WORKDIR /app/aspnetapp
RUN dotnet publish -c Release -o out

# 使用 .NET 8 运行时运行
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/aspnetapp/out ./

# 暴露端口（默认 ASP.NET Core 监听 8080）
EXPOSE 8080

# 启动
ENTRYPOINT ["dotnet", "aspnetapp.dll"]
