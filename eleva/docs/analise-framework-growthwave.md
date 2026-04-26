# Análise do Framework GrowthWave para MCP-RH

## Resumo executivo

O GrowthWave não é um exemplo de Clean Architecture “pura”; o código real é um monólito modular pragmático com `Shared`, `Services`, `Server`, `Integrations.*` e testes. Mesmo assim, ele entrega um conjunto muito bom de padrões reaproveitáveis para o MCP-RH: MCP registry/handler, contexto de tenant por request, guards de persistência, serviços finos por domínio, jobs Quartz, observabilidade por execução e bridge de instalação.

O ponto mais importante para o MCP-RH é este: o padrão é reutilizável, mas o eixo multi-tenant precisa migrar de `InstanceId:int` para `TenantId:Guid` (ou equivalente do nosso modelo), com `X-Tenant-Id` e claims JWT alinhadas aos docs do projeto.

### Arquivos analisados

- [Program.cs](/home/headless/workspace/projeto-sistema/referencia/GrowthWave-main/GrowthWave-main/src/GrowthWave.Server/Program.cs)
- [McpController.cs](/home/headless/workspace/projeto-sistema/referencia/GrowthWave-main/GrowthWave-main/src/GrowthWave.Server/Controllers/McpController.cs)
- [AppDbContext.cs](/home/headless/workspace/projeto-sistema/referencia/GrowthWave-main/GrowthWave-main/src/GrowthWave.Services/Data/AppDbContext.cs)
- [InstanceContextMiddleware.cs](/home/headless/workspace/projeto-sistema/referencia/GrowthWave-main/GrowthWave-main/src/GrowthWave.Server/Middleware/InstanceContextMiddleware.cs)
- [ExecutionStatusService.cs](/home/headless/workspace/projeto-sistema/referencia/GrowthWave-main/GrowthWave-main/src/GrowthWave.Services/Services/ExecutionStatus/ExecutionStatusService.cs)
- [SetupController.cs](/home/headless/workspace/projeto-sistema/referencia/GrowthWave-main/GrowthWave-main/src/GrowthWave.Server/Controllers/SetupController.cs)
- [bridge/index.js](/home/headless/workspace/projeto-sistema/referencia/GrowthWave-main/GrowthWave-main/bridge/index.js)
- [gw-mcp-create-service/SKILL.md](/home/headless/workspace/projeto-sistema/referencia/GrowthWave-main/GrowthWave-main/skills/gw-mcp-create-service/SKILL.md)
- [gw-database-migrations/SKILL.md](/home/headless/workspace/projeto-sistema/referencia/GrowthWave-main/GrowthWave-main/skills/gw-database-migrations/SKILL.md)

## 1. Como o GrowthWave se organiza

### Camadas reais

O solution layout do GrowthWave é composto por:

- `GrowthWave.Shared`: entidades base, interfaces, enums e contratos comuns.
- `GrowthWave.Services`: serviços de domínio, EF Core `AppDbContext`, migrations e helpers.
- `GrowthWave.Server`: ASP.NET Core, controllers, middleware, MCP, jobs, bootstrap e bridge.
- `GrowthWave.Integrations.*`: adaptadores de plataforma e contratos de integração.
- `tests/GrowthWave.UnitTests` e `tests/GrowthWave.IntegrationTests`: cobertura por serviço, integração, API e infraestrutura.

### Leitura arquitetural

Na prática, `Server` orquestra HTTP/MCP/jobs, `Services` concentra regra e persistência, `Shared` concentra primitivas, e `Integrations.*` encapsula dependências externas. Isso aparece claramente no `Program.cs`, que registra `DbContext`, serviços, integrações, Quartz e o registry MCP.

## 2. Padrão MCP

### Núcleo

Os blocos centrais são:

- `McpFunction`: descrição da tool, annotation, schema e handler.
- `McpParameter`: metadados de input.
- `ToolAnnotation`: marca `ReadOnly`, `Mutating`, `ExternalAction` e `Dangerous`.
- `IMcpService`: contrato de registro das tools.
- `McpServiceRegistry`: catálogo em memória das tools.
- `McpArgs`: parser resiliente para `JsonElement` e tipos primitivos.

### Fluxo

1. `Program.cs` instancia e registra os `*McpService`.
2. O registry recebe todas as tools.
3. `McpController` expõe `/mcp/tools` e `/mcp/tools/call`.
4. O controller valida API key, tenant/instance, schema e tool habilitada.
5. O handler resolve dependências via `IServiceProvider` e chama o serviço de domínio.

### Observação importante

O `McpController` também faz auditoria, bloqueio de tools desabilitadas e um guard específico para chamadas raw perigosas. Para o MCP-RH, a estrutura é aproveitável, mas o guard deve ser redesenhado para regras de RH/LGPD, não para marketplace.

## 3. Multi-tenancy

### Como funciona no GrowthWave

- `InstanceContext` guarda `InstanceId` e `UserId`.
- `InstanceContextMiddleware` lê `X-Instance-Id` e `X-User-Id`.
- `ScopedInstanceAccessor` adapta o contexto para `ICurrentInstanceAccessor`.
- `AppDbContext` aplica guards em `SaveChanges` e força `InstanceId` em entidades multi-tenant.

### O que isso significa para MCP-RH

- O mecanismo é reaproveitável.
- O identificador deve virar `TenantId`.
- O header deve virar `X-Tenant-Id`.
- O tipo ideal no MCP-RH é `Guid`/UUID, alinhado aos docs de API e modelo de dados.

### Padrão de persistência

O GrowthWave protege escrita cross-instance no `AppDbContext`: se a entidade implementa `IInstanceBaseEntity`, o contexto popula `InstanceId` automaticamente ou falha. Esse é o principal padrão a copiar para o MCP-RH.

## 4. Persistence

### Como o GrowthWave modela persistência

- Entidades em `GrowthWave.Shared.PersistenceObjects.*`.
- Base comum com `BaseEntity` e `AuditableEntity`.
- `AppDbContext` com `DbSet` por agregado.
- Índices, conversões de enum, relacionamentos e `HasQueryFilter` para soft delete.
- Migrations versionadas em `GrowthWave.Services/Migrations`.
- Startup aplica `Database.Migrate()` fora de teste e `EnsureCreated()` em `Testing`.

### Leitura para MCP-RH

O padrão de “entidade base + guards + migrations no startup” é bom para o RH. O que precisa mudar é:

- `InstanceId` -> `TenantId`.
- `ReferenceCode` vira um identificador útil para idempotência e integrações.
- Os POs devem ficar no domínio/persistência do MCP-RH, não no `Shared` do jeito que o GrowthWave faz.

## 5. Services layer

O GrowthWave usa serviços finos sobre `DbContext`, sem CQRS formal em todo o fluxo. O padrão é:

- método recebe `instanceId`;
- filtra por tenant/instância;
- valida existência e status;
- ajusta estado;
- chama `SaveChangesAsync()`.

Exemplos: `ConfigService`, `ConnectionService`, `TaskService`, `PricingService`, `HistoryService`.

Para o MCP-RH, isso funciona bem para o MVP, desde que:

- a lógica de negócio pesada fique em Application/Domain;
- os serviços de MCP permaneçam finos;
- a regra de acesso por tenant nunca fique implícita.

## 6. Skills

As skills do GrowthWave ficam em `skills/gw-*` e usam frontmatter YAML simples:

- `name`
- `description`

Depois do frontmatter, cada skill segue com:

- quando usar;
- arquivos-chave;
- passo a passo;
- regras obrigatórias;
- armadilhas;
- validação.

Isso é útil como padrão operacional. Para o MCP-RH, a mesma organização pode ser mantida para tarefas como migrations, MCP services, tenancy e compliance.

## 7. Testes

### UnitTests

Cobrem:

- serviços de domínio;
- helpers;
- integrações HTTP com fakes;
- jobs Quartz;
- validação de infraestrutura.

### IntegrationTests

Cobrem:

- API real via `WebApplicationFactory`;
- banco SQLite em memória;
- remoção de hosted services para determinismo;
- seed de `InstancePO` e entidades correlatas;
- controller MCP e endpoints públicos.

### Padrões úteis para MCP-RH

- `TestDbContextFactory`;
- `CustomWebApplicationFactory`;
- builders por agregado;
- `StubHttpClientFactory`;
- testes de guard de tenant/instance no `DbContext`.

## 8. Jobs / Background

O GrowthWave usa `Quartz` no `Program.cs` para agendar jobs como sincronização, métricas, alertas e limpeza. O padrão é declarativo, com cron no bootstrap.

Para o MCP-RH isso se traduz bem em jobs como:

- lembretes de PDI;
- consolidação de performance/BSC;
- cálculo de sinais de engagement;
- geração de resumos de IA;
- expurgo/retencão LGPD;
- auditoria e checks operacionais.

## 9. Execution Status

O padrão é sólido:

- `IExecutionStatusService` define escrita/leitura;
- `ExecutionStatusService` cria um `executionKey` por execução;
- `RedisExecutionStatusStore` armazena outcomes por `instanceId + executionKey`;
- índices separados por execução e por serviço;
- TTL de 24h.

Isso pode ser copiado quase literalmente para o MCP-RH, trocando `instanceId` por `tenantId` e ajustando categorias de serviço para RH/IA.

## 10. Config e Bridge

### Config

O GrowthWave usa `appsettings.json` para:

- connection string;
- Redis;
- storage;
- Playwright;
- integrações externas;
- defaults de negócio.

O MCP-RH deve fazer o mesmo, mas com foco em:

- auth/identity provider;
- OpenAI/Azure OpenAI;
- e-mail;
- storage;
- auditoria/LGPD;
- feature flags por tenant.

### Bridge

O fluxo de bridge é:

1. `SetupController` gera comando de instalação.
2. `InstallController` devolve um script shell com `MCP_INSTANCE_ID`, `MCP_API_KEY`, `MCP_REFERENCE_CODE`, `MCP_BRIDGE_URL` e `MCP_REMOTE_URL`.
3. `BridgePackageController` entrega o pacote `bridge.tgz`.
4. `bridge/index.js` atua como cliente MCP stdio e repassa `list tools` / `call tool` para o backend HTTP.

Isso é reutilizável se o MCP-RH precisar de bridge para clientes locais ou automações externas.

## 11. Mapeamento de adaptação

| GrowthWave | Papel | MCP-RH equivalente | Ação |
|---|---|---|---|
| `GrowthWave.Shared` | Primitivas e contratos | `MCPRH.Shared` + parte de `Domain` | Adaptar namespaces e tipo de tenant |
| `GrowthWave.Services` | Regras + persistência | `MCPRH.Application` + `MCPRH.Infrastructure` | Separar use cases de infra |
| `GrowthWave.Server` | HTTP, MCP, jobs | `MCPRH.WebAPI` | Reimplementar controllers e bootstrap |
| `GrowthWave.Integrations.*` | Adaptadores de marketplaces | `MCPRH.Integrations.*` opcionais | Só criar o que tiver provider real |
| `InstanceContext` | Contexto por request | `TenantContext` | Trocar `InstanceId` por `TenantId` |
| `InstanceContextMiddleware` | Resolve tenant/header | `TenantContextMiddleware` | Ler `X-Tenant-Id` / claim |
| `ScopedInstanceAccessor` | Acesso ao tenant atual | `ScopedTenantAccessor` | Adaptar para `Guid` |
| `BaseEntity` / `AuditableEntity` | Base persistida | `BaseEntity` / `AuditableEntity` do RH | Ajustar `TenantId` e nomes |
| `AppDbContext` | EF Core + guards | `AppDbContext` do RH | Filtro global e guards por tenant |
| `McpController` | Endpoint MCP | `McpController` do RH | Reaproveitar forma, mudar domínio |
| `McpFunction` / `McpArgs` | Tool model e parse | Igual | Reuso quase literal |
| `McpServiceRegistry` / `IMcpService` | Registro de tools | Igual | Reuso quase literal |
| `ExecutionStatusService` | Observabilidade operacional | Igual, com nomes RH | Reuso com ajuste de chave |
| `Quartz` jobs | Background | `Quartz` jobs RH | Reposicionar para PDI/performance/engagement |
| Bridge shell/Node | Transporte MCP | Bridge do MCP-RH | Reusar se houver cliente externo |

## 12. O que pode ser reaproveitado ipsis literis

Reuso direto ou quase direto:

- `McpFunction`
- `McpParameter`
- `McpArgs`
- `ToolAnnotation`
- `McpServiceRegistry`
- `IMcpService`
- `McpController` como forma de endpoint
- `ExecutionStatusService` e `RedisExecutionStatusStore` como padrão
- `Quartz` bootstrap e agendamento
- `bridge/index.js` como cliente stdio
- `skills/` com frontmatter YAML

Reuso com renomeação de domínio:

- `InstanceContext` -> `TenantContext`
- `ScopedInstanceAccessor` -> `ScopedTenantAccessor`
- `InstanceContextMiddleware` -> `TenantContextMiddleware`
- `BaseEntity` / `AuditableEntity`
- `AppDbContext` guard pattern

## 13. O que precisa ser adaptado

- `InstanceId:int` -> `TenantId:Guid`.
- `X-Instance-Id` -> `X-Tenant-Id`.
- namespaces `GrowthWave.*` -> `MCPRH.*`.
- entidades de catálogo/e-commerce -> entidades de RH.
- services de marketplace/pricing/rendering -> services de people, DISC, PDI, performance, engagement e IA.
- regras de auditoria -> LGPD e trilhas de decisão.
- bridge/install -> onboarding do tenant e distribuição do cliente MCP.

## 14. O que não se aplica

- integrações de marketplace: Bling, Tray, Shopee, Mercado Livre, Nuvemshop, Omie;
- catálogo, imagem, vídeo e renderização;
- pricing, margin limits, channel pricing e experimentos de preço;
- pedidos, clientes e sincronização de vendas;
- approval workflow de produto;
- task/public pages;
- qualquer lógica específica de e-commerce.

## 15. Estrutura proposta de projetos `.csproj`

### Base recomendada

```text
src/
├── MCPRH.Shared/
├── MCPRH.Domain/
├── MCPRH.Application/
├── MCPRH.Infrastructure/
├── MCPRH.WebAPI/
tests/
├── MCPRH.UnitTests/
└── MCPRH.IntegrationTests/
```

### Dependências sugeridas

- `MCPRH.Shared`: sem dependências.
- `MCPRH.Domain`: depende de `Shared` apenas se necessário.
- `MCPRH.Application`: depende de `Domain` e `Shared`.
- `MCPRH.Infrastructure`: depende de `Application`, `Domain`, `Shared`.
- `MCPRH.WebAPI`: depende de `Application` e `Infrastructure`.
- `MCPRH.UnitTests`: referencia `Application`, `Domain`, `Shared`.
- `MCPRH.IntegrationTests`: referencia `WebAPI`.

### Extensões opcionais

Se houver justificativa real, separar depois:

- `MCPRH.Integrations.OpenAI`
- `MCPRH.Integrations.Identity`
- `MCPRH.Integrations.Email`
- `MCPRH.Integrations.Storage`

## 16. MCP Services a criar

### Núcleo do produto

- `PeopleMcpService`
- `DiscMcpService`
- `PdiMcpService`
- `PerformanceMcpService`
- `EngagementMcpService`
- `AiCopilotMcpService`

### Sugestão de toolset inicial

- `people_list`, `people_get`, `people_create`, `people_update`, `people_search`
- `disc_assessment_create`, `disc_assessment_submit`, `disc_result_get`, `disc_team_map`
- `pdi_plan_create`, `pdi_plan_get`, `pdi_plan_list`, `pdi_goal_create`, `pdi_action_create`, `pdi_checkpoint_create`
- `bsc_goal_create`, `bsc_goal_list`, `bsc_indicator_create`, `bsc_indicator_update`, `performance_review_create`
- `pulse_create`, `pulse_respond`, `pulse_results`, `feedback_create`, `checkin_create`, `checkin_list`
- `ai_pdi_recommend`, `ai_disc_insights`, `ai_team_analysis`, `ai_leadership_tips`, `ai_summary_generate`

### Cross-cutting opcional

- `IdentityTenantMcpService`
- `ConfigMcpService`
- `AuditMcpService`
- `GovernanceMcpService`

## 17. Ordem recomendada de implementação

1. `TenantId` e base persistence.
2. `AppDbContext`, filtros globais, guards e migrations.
3. Identity/Tenant e autorização.
4. People.
5. PDI.
6. DISC.
7. Performance/BSC.
8. Engagement.
9. AI Copilot.
10. MCP controller, registry e services.
11. Quartz jobs e observabilidade.
12. Bridge/install e automação de onboarding.
13. Testes unitários e de integração por módulo.

## Conclusão

O maior valor do GrowthWave para o MCP-RH não está no domínio de e-commerce, e sim no conjunto de padrões operacionais: tenancy, registry MCP, guard de persistência, serviços finos, observabilidade e bootstrap prático. O que deve ser copiado é a mecânica; o que deve ser trocado é o domínio, o tipo de tenant e a fronteira de segurança/LGPD.
