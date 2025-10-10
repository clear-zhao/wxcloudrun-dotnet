# 使用 .NET 8 SDK 构建阶段镜像
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

# 使用腾讯源加速 apk 下载
RUN sed -i 's/dl-cdn.alpinelinux.org/mirrors.tencent.com/g' /etc/apk/repositories

WORKDIR /source

# 先复制解决方案和项目文件
COPY *.sln .
COPY aspnetapp/*.csproj ./aspnetapp/

# 还原依赖
RUN dotnet restore -r linux-musl-x64 /p:PublishReadyToRun=true

# 复制全部源代码并发布
COPY aspnetapp/. ./aspnetapp/
WORKDIR /source/aspnetapp
RUN dotnet publish -c Release -o /app -r linux-musl-x64 \
    --self-contained true --no-restore \
    /p:PublishTrimmed=true /p:PublishReadyToRun=true /p:PublishSingleFile=true

# ===========================
# 运行阶段镜像
# ===========================
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine

# 设置时区为上海
RUN apk add --no-cache tzdata ca-certificates \
    && cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime \
    && echo "Asia/Shanghai" > /etc/timezone

WORKDIR /app

# 拷贝发布输出
COPY --from=build /app .

# 挂载 DataProtection 密钥目录（可选）
VOLUME /root/.aspnet/DataProtection-Keys

# 启动应用
ENTRYPOINT ["./aspnetapp"]
