# Análise — Painéis, Dashboards e Motor de Questionários

> Baseado em leitura direta do código. Data: 2026-04-26.

---

## Estado dos Dashboards e Painéis

**Não existe nenhum `DashboardController`, `ReportController` ou `SummaryController`** no projeto. Todo o acesso a dados agregados é feito exclusivamente via MCP tools expostas em `McpController.cs` (`POST /mcp/tools/call`).

### Mecanismos de agregação existentes

| MCP Tool | Arquivo | O que agrega | Qualidade |
|---|---|---|---|
| `pdi_progress_summary` | `PdiMcpService.cs` | Resumo de progresso de um plano PDI específico | ✅ Chama `IPdiService.GetProgressSummaryAsync` — implementado |
| `mood_team_summary` | `EngagementMcpService.cs` | Últimas 20 entradas de humor de um gestor | ⚠️ **BUG crítico**: filtra `EmployeeId == managerId` duas vezes (OR redundante) — retorna apenas os registros do próprio gestor, **nunca da equipe** |
| `pulse_results` | `EngagementMcpService.cs` | Resultados de uma pesquisa pulse por departamento | ✅ Chama `IPulseService.GetResultsAsync` — implementado |
| `ec_org_maturity` | `EcMcpService.cs` | Maturidade organizacional EC por unidade | ✅ Chama `IEcService.GetOrgMaturityAsync` — implementado |
| `bsc_perspective_list` | `PerformanceMcpService.cs` | Lista perspectivas BSC | ✅ Funcional — pode compor painel de performance |
| `bsc_goal_list` | `PerformanceMcpService.cs` | Metas BSC filtradas por dono, status, perspectiva | ✅ Funcional |
| `bsc_indicator_update` | `PerformanceMcpService.cs` | Atualiza indicador — não é leitura de painel | ⚠️ Mutação, não dashboard |
| `disc_team_map` | `DiscMcpService.cs` | Mapa DISC de uma equipe por gestor | ✅ Funcional |
| `ai_team_analysis` | `AiCopilotMcpService.cs` | Análise de equipe por IA | ✅ Chama `IAiRecommendationService.TeamAnalysisAsync` |

### Conclusão sobre dashboards

O sistema **não tem UI de painel** — é inteiramente API/MCP. As tools existentes funcionam como blocos de dados que um frontend ou agente de IA deve compor. O BSC (`bsc_perspective_list` + `bsc_goal_list`) é o conjunto mais próximo de um painel de performance navegável. O `mood_team_summary` está com bug e entrega dados errados.

---

## Motor de Questionários — Estado Atual

### Estrutura do banco de perguntas

**Arquivo:** `src/Eleva.Services/Services/Questions/QuestionBankService.cs`
**Interface:** `IQuestionBankService`

| Método | Implementação real? | Detalhe |
|---|---|---|
| `ListBanksAsync(instanceId)` | ✅ | Filtra por `InstanceId`, `IsActive`, soft-delete |
| `ListQuestionsAsync(instanceId, bankId?, search?)` | ✅ | Busca em `Text` e `HelpText`, ordena por `SortOrder` |
| `CreateBankAsync(instanceId, bank)` | ✅ | Força `IsActive = true` |
| `CreateQuestionAsync(instanceId, question)` | ⚠️ | Não stampa `InstanceId` no entity — espera que venha preenchido pelo chamador |
| `SelectSmartAsync(instanceId, contextType, contextId, take)` | ❌ **Stub disfarçado** | Ignora `contextType` e `contextId` completamente. Faz apenas `OrderBy(SortOrder).Take(take)` — sem lógica de seleção por contexto |
| `ValidateCrossAsync(instanceId, questionId, targetBankId?)` | ⚠️ | Funcional mas trivial: verifica apenas se `QuestionBankId == targetBankId`. Sem validação semântica |

**Modelo de dados — `QuestionPO`:**
- `QuestionBankId` (FK), `Code`, `Text`, `HelpText`, `QuestionType` (enum), `AnswerFormat` (enum), `IsRequired`, `SortOrder`, `MetadataJson`, soft-delete.

**Modelo de dados — `AnswerPO`:**
- Suporta `TextValue`, `NumericValue`, `BooleanValue`, `JsonValue`, `Score` — estrutura polimórfica genérica.

### Reutilização entre módulos — DISC / EC / Pulse / CheckIn

| Módulo | Usa QuestionBank? | Mecanismo de pergunta | Vínculo |
|---|---|---|---|
| **DISC** | ❌ Não | `DiscResponsePO.QuestionCode` (string livre, ex: `"D1"`, `"I2"`) | Zero vínculo com `QuestionBankPO` — perguntas são implícitas no código do serviço |
| **EC** | ⚠️ Parcial | `EcResponsePO.QuestionId` (int?, opcional) | FK existe mas é nullable — respostas podem ser gravadas sem nenhuma pergunta vinculada |
| **Pulse** | ⚠️ Dual-path | `PulseSurveyPO.QuestionsJson` (blob) **E** `PulseQuestionPO` (entidade separada com FK) | Dois mecanismos paralelos — risco de inconsistência entre o JSON e as entidades |
| **CheckIn** | ❌ Não | Campos fixos: `Mood`, `EnergyLevel`, `ProductivityScore`, `Blockers`, `Notes` | Modelo fechado, sem perguntas configuráveis |
| **Feedback** | ❌ Não | Texto livre em `Content` + enums `FeedbackType` e `Visibility` | Sem banco de perguntas |
| **Review (360°)** | ⚠️ Parcial | `ReviewResponsePO` não inspecionado diretamente, mas tool `review_create` aceita `responses[]` | Estrutura genérica, provavelmente sem vínculo com QuestionBank |

**Conclusão:** O `QuestionBank` existe como infraestrutura mas **nenhum módulo principal o utiliza de forma efetiva**. DISC ignora completamente. EC e Pulse têm FKs parciais. O motor é uma fundação não conectada.

---

## Prontidão Operacional

| Fluxo | Está pronto? | O que falta |
|---|---|---|
| **DISC end-to-end** | ⚠️ Parcial | Tools existem (`disc_assessment_create`, `disc_assessment_submit`, `disc_result_get`). **Bloqueio crítico:** `AuthMcpService` é 100% stub (`NotImplementedException`) — sem auth funcional, qualquer chamada exige que `InstanceId` seja injetado por fora (hardcoded ou bypass). `DiscResponsePO.QuestionCode` é string livre — nenhuma validação de que as respostas correspondem a perguntas DISC reais. Falta banco de perguntas DISC cadastrado. |
| **Pulse Survey** | ⚠️ Parcial | `pulse_create`, `pulse_respond`, `pulse_results` funcionam. Problema: `PulseSurveyPO` tem dual-path (`QuestionsJson` vs `PulseQuestionPO`) — criar via MCP popula um, mas qual? Pode gerar surveys sem perguntas estruturadas. Sem auth funcional. |
| **EC Maturidade** | ⚠️ Parcial | `ec_assessment_create`, `ec_assessment_submit`, `ec_result_get`, `ec_org_maturity` implementados. `EcResponsePO.QuestionId` é nullable — respostas podem ser gravadas sem vínculo a perguntas. Sem definição de quais perguntas compõem uma avaliação EC (não há seed ou template de questionário EC). |
| **PDI Dashboard** | ⚠️ Parcial | `pdi_progress_summary` funciona. Não há agregação multi-plano (sem visão de toda a equipe/empresa). Falta tool tipo `pdi_team_summary` ou `pdi_cycle_report`. |
| **BSC Performance** | ✅ Funcional | `bsc_perspective_list` + `bsc_goal_list` (com filtros) + `bsc_indicator_update` cobrem o ciclo básico de gestão por BSC. É o módulo mais completo como painel. |
| **Auth / Instâncias** | ❌ Não implementado | `auth_login`, `auth_refresh`, `instance_create`, `instance_list` são stubs explícitos lançando `NotImplementedException`. O sistema depende de `InstanceId` injetado via `InstanceContext` — sem saber como isso é populado em produção, qualquer fluxo pode falhar silenciosamente com `InstanceId = 0`. |

---

## Bugs Identificados no Código

| Bug | Localização | Impacto |
|---|---|---|
| `mood_team_summary` filtra só o gestor, não a equipe | `EngagementMcpService.cs` — `.Where(x => x.EmployeeId == managerId \|\| x.EmployeeId == managerId)` | Alto — feature completamente quebrada |
| `review_360_create` passa `reviewerId = 0` sempre | `PerformanceMcpService.cs` — parâmetro não declarado em `McpParameter` | Alto — impossível identificar avaliador 360 |
| `SelectSmartAsync` ignora `contextType` e `contextId` | `QuestionBankService.cs` | Médio — feature anunciada como "smart" é apenas `Take(n)` |
| `CreateQuestionAsync` não stampa `InstanceId` | `QuestionBankService.cs` | Médio — questões podem ser criadas sem InstanceId, vazando entre tenants |
| `ICurrentInstanceAccessor` injetado mas nunca usado | `QuestionBankService.cs` | Baixo — dead code, possível resíduo de refatoração |

---

## Recomendações — O que implementar primeiro

### Prioridade 1 — Desbloqueadores críticos

1. **Implementar `AuthMcpService`** — sem `auth_login` e `instance_create` funcionando, nenhum fluxo pode ser testado de forma isolada. É a fundação de tudo.

2. **Corrigir `mood_team_summary`** — mudar o filtro para buscar subordinados do gestor (via `ManagerId` em `EmployeePO` ou join com hierarquia).

### Prioridade 2 — Tornar os fluxos principais utilizáveis

3. **Seed de perguntas DISC** — criar um `QuestionBank` com as perguntas DISC padrão e vincular `DiscResponsePO` ao banco. Atualmente é impossível validar se as respostas são válidas.

4. **Resolver dual-path do Pulse** — escolher entre `QuestionsJson` e `PulseQuestionPO`. Recomendação: deprecar `QuestionsJson`, usar apenas a entidade relacional. Migrar `pulse_create` para popular `PulseQuestionPO`.

5. **Template de questionário EC** — definir quais perguntas compõem uma avaliação EC e criar mecanismo de seed/template (banco de perguntas padrão por tipo de avaliação).

### Prioridade 3 — Completar o motor de perguntas

6. **Implementar `SelectSmartAsync` de verdade** — adicionar lógica de filtro por `contextType` (ex: perguntas de banco DISC vs EC vs Pulse) e ranking por relevância.

7. **Conectar módulos ao QuestionBank** — DISC, EC e Review devem usar `QuestionId` como FK real (não nullable, não string code). Isso habilita rastreabilidade e reuso.

8. **Adicionar `pdi_team_summary`** — tool que agrega progresso de todos os PDIs de uma equipe/ciclo para completar o dashboard PDI.

### Prioridade 4 — Dashboard real

9. **Criar uma tool `dashboard_overview`** — agrega: maturidade EC, progresso PDI médio, NPS do último Pulse, humor médio da equipe, metas BSC abertas. Essa tool seria o painel executivo que hoje não existe.
