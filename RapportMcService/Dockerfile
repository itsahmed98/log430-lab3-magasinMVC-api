# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY *.sln .
COPY RapportMcService/*.csproj ./RapportMcService/
RUN dotnet restore
COPY . .
WORKDIR /app/RapportMcService
RUN dotnet publish -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "RapportMcService.dll"]
