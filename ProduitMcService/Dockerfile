# Étape 1 : build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier uniquement le projet ProduitMcService
COPY ProduitMcService/*.csproj ./ProduitMcService/
WORKDIR /src/ProduitMcService
RUN dotnet restore

# Copier le reste du code
COPY ProduitMcService/. ./ 

# Publier l'application
RUN dotnet publish -c Release -o /app/publish

# Étape 2 : image finale runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ProduitMcService.dll"]
