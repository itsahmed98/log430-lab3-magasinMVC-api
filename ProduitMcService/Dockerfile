# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copier la solution et tous les projets
COPY . .

# Restaurer les dépendances
RUN dotnet restore MagasinCentral.sln

# Publier un projet spécifique (exemple : ProduitMcService)
RUN dotnet publish ProduitMcService/ProduitMcService.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ProduitMcService.dll"]
