# 使用 .NET 9 SDK 构建镜像（官方 Debian 版，兼容性好）
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# 如果你希望使用 Alpine 版本（体积更小但不太兼容），可以换成：
# FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine

# 镜像源加速（如果使用 alpine）
RUN sed -i 's/dl-cdn.alpinelinux.org/mirrors.tencent.com/g' /etc/apk/repositories || true

WORKDIR /source

# 拷贝解决方案和项目文件
COPY *.sln .
COPY aspnetapp/*.csproj ./aspnetapp/

# 先还原依赖
RUN dotnet restore -r linux-x64 /p:PublishReadyToRun=true

# 拷贝项目源代码
COPY aspnetapp/. ./aspnetapp/
WORKDIR /source/aspnetapp

# 发布应用（单文件自包含）
RUN dotnet publish -c Release -o /app -r linux-x64 --self-contained true --no-restore /p:PublishTrimmed=true /p:PublishSingleFile=true

# ---------------------------
# 运行阶段
# ---------------------------
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0

# 时区（可选：使用上海时间）
# RUN apt-get update && apt-get install -y tzdata && \
#     ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime && \
#     echo "Asia/Shanghai" > /etc/timezone

# 安装 HTTPS 证书
RUN apt-get update && apt-get install -y ca-certificates

WORKDIR /app
COPY --from=build /app ./

# 如果需要支持多语言（如中文排序、拼音等）
# ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
# RUN apt-get install -y libicu-dev

ENTRYPOINT ["./aspnetapp"]
