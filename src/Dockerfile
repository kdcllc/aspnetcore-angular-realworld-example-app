# BUILD IMAGE: docker build . -f "src\Dockerfile" -t "conduit:latest"
# RUNNING THE IMAGE docker run --rm -it -p 5000:5000 conduit:latest

FROM microsoft/dotnet:2.1.2-aspnetcore-runtime AS base
#install ps
RUN apt-get update && apt-get install -y procps

# Debugger
WORKDIR /vsdbg
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    unzip \
    && rm -rf /var/lib/apt/list/* \
    && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg
# END Debugger

# NODE
ENV NODE_VERSION 8.11.3
ENV NODE_DOWNLOAD_URL https://nodejs.org/dist/v$NODE_VERSION/node-v$NODE_VERSION-linux-x64.tar.gz
# https://nodejs.org/download/release/v8.11.3/SHASUMS256.txt
ENV NODE_DOWNLOAD_SHA 1ea408e9a467ed4571730e160993f67a100e8c347f6f9891c9a83350df2bf2be

RUN curl -SL "$NODE_DOWNLOAD_URL" --output nodejs.tar.gz \
    && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
    && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
    && rm nodejs.tar.gz \
    && ln -s /usr/local/bin/node /usr/local/bin/nodejs
# END NODE

WORKDIR /app
EXPOSE 5000

# https://github.com/aspnet/aspnet-docker/tree/master/2.1
FROM microsoft/dotnet:2.1-sdk AS build

# NODE
ENV NODE_VERSION 8.11.3
ENV NODE_DOWNLOAD_URL https://nodejs.org/dist/v$NODE_VERSION/node-v$NODE_VERSION-linux-x64.tar.gz
# https://nodejs.org/download/release/v8.11.3/SHASUMS256.txt
ENV NODE_DOWNLOAD_SHA 1ea408e9a467ed4571730e160993f67a100e8c347f6f9891c9a83350df2bf2be

RUN curl -SL "$NODE_DOWNLOAD_URL" --output nodejs.tar.gz \
    && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
    && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
    && rm nodejs.tar.gz \
    && ln -s /usr/local/bin/node /usr/local/bin/nodejs
# END NODE

WORKDIR /src
COPY src/Conduit.csproj src/
RUN dotnet restore src/Conduit.csproj
COPY . .
WORKDIR /src/src

RUN dotnet build Conduit.csproj -c Debug -o /app

FROM build AS publish
RUN dotnet publish Conduit.csproj -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Conduit.dll"]
