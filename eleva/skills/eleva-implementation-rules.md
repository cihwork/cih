# Eleva — Regras de Implementação

## Namespaces
- Sempre `Eleva.*` — nunca reutilizar namespace de outro produto
- `Eleva.Shared` — interfaces, enums, POs
- `Eleva.Services` — AppDbContext, serviços de domínio
- `Eleva.Server` — HTTP, MCP, middleware, jobs

## Entidades
- Toda entidade herda `BaseEntity` (`Eleva.Shared.Interfaces`)
- Toda entidade multi-tenant tem `InstanceId` preenchido automaticamente
- `InstanceId` sempre aplicado em queries e escritas — nunca cruzar tenant
- Soft delete via `DeletedAt` onde aplicável — usar `HasQueryFilter` para esconder

## Services
- Construtores recebem `AppDbContext` e `ICurrentInstanceAccessor`
- `InstanceId` vem do parâmetro do método, nunca do accessor direto nos services
- Transactions explícitas para operações que escrevem em múltiplas tabelas
- Enums persistem como string no banco

## MCP Services
- Implementam `IMcpService` com método `RegisterTools(McpServiceRegistry registry)`
- Cada tool é um `McpFunction` com `Name`, `Description`, `Annotation`, `Parameters`, `Handler`
- `ToolAnnotation`: ReadOnly, Mutating, ExternalAction, Dangerous
- Handler resolve services via `sp.GetRequiredService<>()` — nunca capturar em closure
- Usar `McpArgs` para parse: `StrOrNull`, `IntOrNull`, `Bool`, `DecimalOrNull`, `Int`, etc.
- Resolver `InstanceContext` via `sp.GetRequiredService<InstanceContext>().InstanceId`

## Build
- `dotnet build Eleva.sln` deve passar com **0 warnings e 0 errors**
- Nunca commitar com build quebrado
- Após mudar POs, gerar nova migration antes de subir

## Padrão arquitetural
- Seguir o Escalada Framework — não inventar padrões novos
- 3 camadas: Server / Services / Shared
- Não criar helpers globais sem necessidade clara
