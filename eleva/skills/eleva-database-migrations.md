# Eleva — Migrations EF Core

## Gerar nova migration

```bash
dotnet ef migrations add NomeDaMigration \
  --project src/Eleva.Services/Eleva.Services.csproj \
  --startup-project src/Eleva.Server/Eleva.Server.csproj \
  --output-dir Data/Migrations
```

## Aplicar migration

```bash
dotnet ef database update \
  --project src/Eleva.Services/Eleva.Services.csproj \
  --startup-project src/Eleva.Server/Eleva.Server.csproj
```

## Rollback

```bash
dotnet ef database update NomeMigrationAnterior \
  --project src/Eleva.Services/Eleva.Services.csproj \
  --startup-project src/Eleva.Server/Eleva.Server.csproj
```

## Listar migrations aplicadas

```bash
dotnet ef migrations list \
  --project src/Eleva.Services/Eleva.Services.csproj \
  --startup-project src/Eleva.Server/Eleva.Server.csproj
```

## Fluxo completo ao adicionar um PO novo

1. Criar o PO em `src/Eleva.Shared/PersistenceObjects/Xxx/XxxPO.cs` herdando `BaseEntity`
2. Adicionar `DbSet<XxxPO> Xxxs => Set<XxxPO>();` em `AppDbContext`
3. Gerar migration
4. Revisar o arquivo gerado em `src/Eleva.Services/Data/Migrations/`
5. Aplicar migration
6. `dotnet build Eleva.sln` — 0 errors

## Config de conexão (appsettings.json)

- **Dev (SQLite):** `"DefaultConnection": "Data Source=database/eleva.db"`
- **Prod (MySQL):** `"DefaultConnection": "Server=...;Database=eleva;User=...;Password=..."`
- **MySQL version:** configurar em `"MySql:ServerVersion": "11.0.0-mariadb"`
