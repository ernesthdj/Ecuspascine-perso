# ===== build stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier les .csproj (API + Contracts) pour un restore plus rapide via le cache
COPY api/Ecauspacine.Api/Ecauspacine.Api.csproj api/Ecauspacine.Api/
COPY shared/Ecauspacine.Contracts/Ecauspacine.Contracts.csproj shared/Ecauspacine.Contracts/

# Restore
RUN dotnet restore api/Ecauspacine.Api/Ecauspacine.Api.csproj

# Copier le reste du dépôt (le contexte de build doit être la racine du repo)
COPY . .

# Publish
RUN dotnet publish api/Ecauspacine.Api/Ecauspacine.Api.csproj -c Release -o /out

# ===== runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out ./
ENV ASPNETCORE_URLS=http://0.0.0.0:5000 \
    ASPNETCORE_ENVIRONMENT=Production
EXPOSE 5000
ENTRYPOINT ["dotnet","Ecauspacine.Api.dll"]
