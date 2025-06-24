# Étape 1 : build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY VenteMcService/*.csproj ./VenteMcService/
WORKDIR /src/VenteMcService
RUN dotnet restore

COPY VenteMcService/. ./
RUN dotnet publish -c Release -o /app/publish

# Étape 2 : runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "VenteMcService.dll"]
