# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY MagasinCentral/*.csproj ./MagasinCentral/
WORKDIR /src/MagasinCentral
RUN dotnet restore

COPY MagasinCentral/. ./
RUN dotnet publish -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MagasinCentral.dll"]
