# ===========================
# 构建阶段
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

# 使用腾讯源加速 apk 下载
RUN sed -i 's/dl-cdn.alpinelinux.org/mirrors.tencent.com/g' /etc/apk/repositories

# 安装时区、证书、依赖
RUN apk add --no-cache tzdata icu-libs krb5-libs libgcc libstdc++ zlib

# 设置工作目录
WORKDIR /source

# 复制解决方案和项目文件
COPY *.sln ./
COPY aspnetapp/*.csproj ./aspnetapp/

# 还原依赖（针对 Alpine）
RUN dotnet restore ./aspnetapp/aspnetapp.csproj -r linux-musl-x64

# 复制源代码
COPY aspnetapp/. ./aspnetapp/
WORKDIR /source/aspnetapp

# 进行发布（非 self-contained 模式，避免 GC 初始化错误）
RUN dotnet publish -c Release -o /app -r linux-musl-x64 \
    --no-self-contained --no-restore \
    /p:PublishTrimmed=false /p:PublishReadyToRun=true /p:PublishSingleFile=true

# ===========================
# 运行阶段
# ===========================
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine

# 安装时区和根证书
RUN apk add --no-cache tzdata ca-certificates \
    && cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime \
    && echo "Asia/Shanghai" > /etc/timezone

# 启用 ICU 支持（解决部分本地化问题）
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app

# 从构建阶段拷贝输出
COPY --from=build /app ./

# 可选：挂载 ASP.NET Core 数据保护密钥目录
VOLUME /root/.aspnet/DataProtection-Keys

# 启动应用
ENTRYPOINT ["./aspnetapp"]
