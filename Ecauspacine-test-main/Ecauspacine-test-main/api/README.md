# Backend — API .NET (C#)

API REST pour EcausPacine (ASP.NET Core).

## Prérequis

- .NET 8 SDK (ou version alignée avec la promo)
- EF Core + provider MySQL (Pomelo)
- Accès DB via **tunnel SSH** (voir `../scripts/README.md`)

## Configuration — connexions DB

### DEV (via tunnel)
`appsettings.Development.json` :
```json
{
  "ConnectionStrings": {
    "Default": "Server=127.0.0.1;Port=3307;Database=ecauspacine_dev;User ID=dev_team;Password=<MDP_DEVTEAM>;TreatTinyAsBoolean=false"
  },
  "Kestrel": {
    "Endpoints": {
      "Http": { "Url": "http://127.0.0.1:5000" }
    }
  }
}
```

> Ouvrez le tunnel (voir `../scripts/README.md`) puis consommez l’API via **http://localhost:5001** (forward du port 5000).

### PROD (sur VPS, dans Docker réseau)
`appsettings.Production.json` (exemple) :
```json
{
  "ConnectionStrings": {
    "Default": "Server=db;Port=3306;Database=ecauspacine_prod;User ID=db_admin;Password=<MDP_ADMIN>;TreatTinyAsBoolean=false"
  }
}
```

## EF Core — Migrations

```bash
# Exemple
dotnet ef migrations add Init --project EcausPacine.Api
dotnet ef database update --project EcausPacine.Api
```

## Lancer en DEV

```bash
dotnet run --project backend/EcausPacine.Api
# Kestrel écoute 127.0.0.1:5000 ; via tunnel → http://localhost:5001
```

## Déploiement (idée générale)

- Builder/publier l’API (`dotnet publish -c Release`)  
- Conteneuriser si besoin (Dockerfile + compose), exposer en **127.0.0.1:5000** côté VPS et reverse-proxy si public.
- Variables de prod via variables d’environnement ou secrets docker.
