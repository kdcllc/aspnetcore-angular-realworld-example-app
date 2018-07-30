FROM microsoft/dotnet:2.1.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1.2-sdk AS build
WORKDIR /src
COPY src/Conduit.csproj src/
RUN dotnet restore src/Conduit.csproj
COPY . .
WORKDIR /src/src
RUN dotnet build Conduit.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Conduit.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Conduit.dll"]
