# Lacunas Funcionais — People, DISC, EC, PDI, Performance, BSC

## Resumo Executivo

O sistema Eleva possui estrutura de dados e interfaces bem definidas, mas a maioria dos campos interpretativos/analíticos dos POs nunca é preenchida por nenhum serviço. Módulos inteiros previstos na spec (Engagement, AICopilot, Questions, Audit) não existem no código. Há um bug crítico no MCP de Performance onde `reviewerId` nunca chega ao banco, e o BSC é inutilizável em instâncias novas por ausência de seed de perspectivas.

---

## Módulo: People (Employee / Department)

### 1. `employee_create` cria colaborador com status `Terminated` por padrão

- **Impacto**: Alto
- **O que falta**: `McpArgs.Bool(args, "isActive")` retorna `false` quando o parâmetro não é informado, resultando em `EmploymentStatus.Terminated` ao invés de `Active`.
- **Onde**: `src/Eleva.Server/Mcp/Services/PeopleMcpService.cs` — handler `employee_create`
- **Sugestão**: Mudar o default para `true`: `McpArgs.Bool(args, "isActive", defaultValue: true)`

### 2. Tool `department_get` ausente no MCP

- **Impacto**: Médio
- **O que falta**: `IDepartmentService.GetAsync` existe na interface e na implementação, mas nenhuma tool MCP expõe essa operação. Impossível buscar um departamento por ID via MCP.
- **Onde**: `src/Eleva.Server/Mcp/Services/PeopleMcpService.cs` (ausência); `src/Eleva.Services/Services/People/IDepartmentService.cs`
- **Sugestão**: Registrar tool `department_get` com parâmetro `departmentId` chamando `IDepartmentService.GetAsync`.

### 3. Gerenciamento de cargos e times somente leitura no MCP

- **Impacto**: Médio
- **O que falta**: Existem `position_list` e `team_list`, mas não há `position_create`, `team_create` ou `team_member_add`. Não é possível criar ou alterar estrutura organizacional via MCP.
- **Onde**: `src/Eleva.Server/Mcp/Services/PeopleMcpService.cs`
- **Sugestão**: Implementar tools de escrita para `PositionPO` e `TeamPO` se os services correspondentes existirem, ou criar `IPositionService`/`ITeamService`.

---

## Módulo: DISC

### 4. `assessmentType` descartado silenciosamente na criação

- **Impacto**: Alto
- **O que falta**: O parâmetro `assessmentType` é recebido em `CreateAssessmentAsync` mas ignorado — o `DiscAssessmentPO` não possui campo para armazená-lo e nenhum serviço o usa.
- **Onde**: `src/Eleva.Services/Services/People/DiscService.cs` — `CreateAssessmentAsync`
- **Sugestão**: Adicionar campo `AssessmentType` ao `DiscAssessmentPO` (migration + atribuição no service).

### 5. Cálculo DISC ignora `AnswerValue` — apenas conta respostas por eixo

- **Impacto**: Alto
- **O que falta**: `SubmitAssessmentAsync` conta `responses.Count(r => r.Axis == "D")` para calcular o score, ignorando completamente `DiscResponsePO.AnswerValue`. Respostas com peso 1 e peso 5 têm o mesmo efeito. Não há normalização.
- **Onde**: `src/Eleva.Services/Services/People/DiscService.cs` — `SubmitAssessmentAsync`
- **Sugestão**: Refatorar o cálculo para usar soma ponderada de `AnswerValue` por eixo, normalizada pelo total.

### 6. Campos interpretativos do `DiscResultPO` nunca preenchidos

- **Impacto**: Alto
- **O que falta**: Os campos `Interpretation`, `StrengthsJson`, `DevelopmentAreasJson`, `CommunicationTipsJson` e `StressIndicatorsJson` são a proposta central do DISC (entregar insights sobre o perfil), mas nenhum serviço os preenche. São sempre `null`.
- **Onde**: `src/Eleva.Services/Services/People/DiscService.cs` — `SubmitAssessmentAsync`; `src/Eleva.Shared/PersistenceObjects/DiscResultPO.cs`
- **Sugestão**: Implementar lógica de interpretação baseada no perfil dominante (D/I/S/C), ou integrar com o módulo AICopilot (quando implementado) para geração dos textos.

### 7. `DiscAssessmentPO.StartedAt`, `AssessmentLink`, `ReportUrl`, `RawDataJson` nunca definidos

- **Impacto**: Médio
- **O que falta**: Quatro campos do PO jamais recebem valor em nenhum serviço.
- **Onde**: `src/Eleva.Shared/PersistenceObjects/DiscAssessmentPO.cs`; `src/Eleva.Services/Services/People/DiscService.cs`
- **Sugestão**: `StartedAt` deve ser definido em `SubmitAssessmentAsync` (ou na primeira resposta); os demais dependem de features a implementar (link externo, upload de relatório).

### 8. Tool `disc_assessment_list` ausente no MCP

- **Impacto**: Médio
- **O que falta**: Impossível listar assessments DISC de um colaborador via MCP — não há como saber quais assessments existem sem acessar o banco diretamente.
- **Onde**: `src/Eleva.Server/Mcp/Services/DiscMcpService.cs`
- **Sugestão**: Registrar tool `disc_assessment_list` com parâmetro `employeeId`.

---

## Módulo: EC (Escuta Clínica / Espiral da Consciência)

### 9. `EcResultPO.MaturityLevel` nunca calculado — dado central do módulo

- **Impacto**: Alto
- **O que falta**: `MaturityLevel` é o output principal do EC (nível na Espiral da Consciência), mas `SubmitAssessmentAsync` nunca o preenche. O campo sempre permanece `null`.
- **Onde**: `src/Eleva.Services/Services/Assessments/EcService.cs` — `SubmitAssessmentAsync`
- **Sugestão**: Implementar mapeamento de `overallScore` para `MaturityLevel` conforme a escala da Espiral da Consciência (Bege, Roxo, Vermelho, Azul, Laranja, Verde, Amarelo, Turquesa).

### 10. `GetOrgMaturityAsync` sempre retorna `AverageMaturityLevel = 0`

- **Impacto**: Alto
- **O que falta**: Consequência direta da lacuna #9 — como `MaturityLevel` é sempre `null`, o filtro `.Where(x => x.MaturityLevel.HasValue)` descarta todos os registros, e `DefaultIfEmpty(0).Average()` retorna sempre `0`.
- **Onde**: `src/Eleva.Services/Services/Assessments/EcService.cs` — `GetOrgMaturityAsync`
- **Sugestão**: Resolver a lacuna #9; o cálculo em si está correto.

### 11. Campos analíticos do `EcResultPO` e `EcAssessmentPO` nunca preenchidos

- **Impacto**: Alto
- **O que falta**: Os seguintes campos jamais recebem valor:
  - `EcResultPO`: `ParticipationRate`, `StrengthsJson`, `RisksJson`, `RecommendationsJson`
  - `EcAssessmentPO`: `ConsciousnessLevel`, `DimensionsJson`, `Findings`, `Recommendations`
- **Onde**: `src/Eleva.Services/Services/Assessments/EcService.cs` — `SubmitAssessmentAsync`
- **Sugestão**: Implementar cálculo de `ParticipationRate` (respostas recebidas / convidados); os campos JSON dependem de lógica de análise a definir.

### 12. Tool `ec_assessment_list` ausente no MCP

- **Impacto**: Médio
- **O que falta**: Sem forma de listar assessments EC de uma instância via MCP.
- **Onde**: `src/Eleva.Server/Mcp/Services/EcMcpService.cs`
- **Sugestão**: Registrar tool `ec_assessment_list`.

---

## Módulo: PDI

### 13. Campos `Progress` de `PdiPlanPO` e `PdiGoalPO` nunca atualizados

- **Impacto**: Alto
- **O que falta**: Os campos `Progress` existem nos POs para indicar o percentual de conclusão, mas nenhum serviço os recalcula após criação/atualização de goals ou actions. Sempre `null`.
- **Onde**: `src/Eleva.Services/Services/Pdi/PdiService.cs` — falta lógica de rollup de progresso
- **Sugestão**: Recalcular `PdiGoalPO.Progress` como média do progresso das actions associadas, e `PdiPlanPO.Progress` como média dos goals, em cada operação que muda status de action.

### 14. Tools de atualização PDI ausentes no MCP

- **Impacto**: Alto
- **O que falta**: Não é possível atualizar status de plano, goal ou action via MCP. `IPdiCycleService.UpdateStatusAsync` existe mas não está exposto. As tools ausentes são:
  - `pdi_plan_update`
  - `pdi_goal_update`
  - `pdi_action_update` (ex: marcar como concluída)
  - `pdi_cycle_update_status`
- **Onde**: `src/Eleva.Server/Mcp/Services/PdiMcpService.cs`
- **Sugestão**: Registrar as 4 tools de atualização.

### 15. `pdi_evidence_upload` sem validação de tipo/tamanho de arquivo

- **Impacto**: Médio
- **O que falta**: A tool aceita qualquer conteúdo como evidência sem validar extensão ou tamanho. Sem restrições, qualquer string (inclusive inválida) é gravada como evidência.
- **Onde**: `src/Eleva.Server/Mcp/Services/PdiMcpService.cs` — handler `pdi_evidence_upload`; `src/Eleva.Services/Services/Pdi/PdiService.cs`
- **Sugestão**: Adicionar validação de tipo MIME/extensão permitida (PDF, JPG, PNG) e limite de tamanho.

---

## Módulo: Performance (BSC / Review)

### 16. `review_360_create` no MCP passa `reviewerId = 0` sempre

- **Impacto**: Crítico
- **O que falta**: O parâmetro `reviewerId` não está declarado nos `Parameters` da tool `review_360_create`, mas o handler tenta lê-lo via `McpArgs.Int(args, "reviewerId", 0)`. Por nunca estar presente nos args, sempre retorna `0`, e todas as reviews 360 são gravadas sem avaliador.
- **Onde**: `src/Eleva.Server/Mcp/Services/PerformanceMcpService.cs` — registro da tool `review_360_create` (parâmetros) e handler (linha ~151)
- **Sugestão**: Adicionar `{ "reviewerId", new McpParameterInfo { ... } }` aos `Parameters` da tool.

### 17. `PerformanceReviewPO.OverallScore` nunca calculado após submit

- **Impacto**: Alto
- **O que falta**: `ReviewService.SubmitAsync` muda o status da review mas não agrega as `ReviewResponsePO.Score` para calcular `OverallScore`. O campo permanece `null` após submissão.
- **Onde**: `src/Eleva.Services/Services/Performance/ReviewService.cs` — `SubmitAsync`
- **Sugestão**: Após mudar status para `Submitted`, calcular `OverallScore = responses.Average(r => r.Score)` e persistir.

### 18. `BscPerspectivePO` sem seed — BSC inutilizável em instâncias novas

- **Impacto**: Alto
- **O que falta**: `IBscService` não tem `CreatePerspectiveAsync`. Não há seed, migration ou job que crie as 4 perspectivas padrão do BSC (Financeira, Clientes, Processos Internos, Aprendizado e Crescimento). `bsc_perspective_list` sempre retorna `[]`, e criar metas BSC exige `PerspectiveId` válido.
- **Onde**: `src/Eleva.Services/Services/Performance/BscService.cs`; `src/Eleva.Server/Mcp/Services/PerformanceMcpService.cs`
- **Sugestão**: Criar seed no `AppDbContext.OnModelCreating` ou em job de bootstrap; adicionar `bsc_perspective_create` ao MCP para cenários customizados.

### 19. `BscGoalPO.Progress` e `PerformanceReviewPO.CalibrationScore`/`ClosedAt` nunca definidos

- **Impacto**: Médio
- **O que falta**:
  - `BscGoalPO.Progress`: nunca calculado por nenhum serviço
  - `PerformanceReviewPO.CalibrationScore`: processo de calibração não implementado
  - `PerformanceReviewPO.ClosedAt`: nunca definido mesmo quando a review é fechada
- **Onde**: `src/Eleva.Services/Services/Performance/BscService.cs`; `src/Eleva.Services/Services/Performance/ReviewService.cs`
- **Sugestão**: `ClosedAt` deve ser atribuído em `SubmitAsync`; `CalibrationScore` depende de feature de calibração a implementar; `BscGoalPO.Progress` deve ser recalculado quando KRs/métricas são atualizados.

### 20. Tool `review_list` ausente no MCP

- **Impacto**: Médio
- **O que falta**: `IReviewService.ListAsync` existe mas não está exposto no MCP.
- **Onde**: `src/Eleva.Server/Mcp/Services/PerformanceMcpService.cs`
- **Sugestão**: Registrar tool `review_list` com parâmetros `employeeId` e `cycleId`.

---

## Problemas Transversais

### 21. `InstanceId` não atribuído explicitamente em criações críticas

- **Impacto**: Alto
- **O que falta**: `DiscService.CreateAssessmentAsync` e `ReviewService.Create360Async` criam entidades sem atribuir `InstanceId`. Em ambiente multi-tenant, se o `AppDbContext` não interceptar `SaveChanges` para injetar o `InstanceId` do contexto atual, os registros serão gravados com `InstanceId = 0`, quebrando o isolamento entre instâncias.
- **Onde**: `src/Eleva.Services/Services/People/DiscService.cs`; `src/Eleva.Services/Services/Performance/ReviewService.cs`
- **Sugestão**: Verificar se o interceptor de `SaveChanges` em `AppDbContext` cobre esses casos; se não, atribuir `InstanceId` explicitamente nos services.

### 22. Auditoria completamente ausente

- **Impacto**: Alto
- **O que falta**: Nenhuma operação mutating registra `AuditLogPO` ou `ChangeLogPO`. Os POs e `IAuditLogService` são previstos na spec mas não implementados. Não há rastreabilidade de quem fez o quê.
- **Onde**: Todos os services e o `McpController`
- **Sugestão**: Implementar `IAuditLogService` e registrar operações CUD via interceptor de EF Core ou middleware de MCP.

### 23. Jobs Quartz previstos na spec mas não implementados

- **Impacto**: Médio
- **O que falta**: Os 6 jobs previstos (`PdiNotificationJob`, `AssessmentCycleJob`, `PulseSurveyJob`, `PerformanceCycleJob`, `AiRecommendationJob`, `AuditConsolidationJob`) são mencionados no `Program.cs` como placeholder mas não existem.
- **Onde**: `src/Eleva.Server/Program.cs` (bootstrap Quartz)
- **Sugestão**: Implementar ao menos `PdiNotificationJob` e `PerformanceCycleJob` como MVP, pois afetam diretamente os módulos existentes.

### 24. Módulos inteiros previstos na spec sem nenhuma implementação

- **Impacto**: Alto
- **O que falta**: Os seguintes módulos têm POs e spec definidos, mas zero implementação de service ou MCP:

| Módulo | POs existentes | MCP tools previstas |
|--------|---------------|---------------------|
| Engagement | `PulseSurveyPO`, `PulseQuestionPO`, `PulseResponsePO`, `FeedbackPO`, `CheckInPO`, `MoodEntryPO` | 9 tools (`pulse_*`, `feedback_*`, `checkin_*`, `mood_*`) |
| AICopilot | `AiRecommendationPO`, `AiInteractionPO` | 5 tools (`ai_pdi_recommend`, `ai_disc_insights`, etc.) |
| Questions | `QuestionBankPO`, `QuestionPO`, `AnswerPO` | 5 tools (`question_*`) |
| Auth/Config | — | `AuthMcpService`, `ConfigMcpService` |
| Audit | `AuditLogPO`, `ChangeLogPO` | `AuditMcpService` |
| System | — | `SystemMcpService` (`system_version`, `system_health`) |

- **Onde**: `src/Eleva.Services/Services/` (ausência); `src/Eleva.Server/Mcp/Services/` (ausência)
- **Sugestão**: Priorizar AICopilot (alto diferencial competitivo) e Auth (bloqueante para segurança).

### 25. `CompetencyLibraryPO` sem seed e sem importação

- **Impacto**: Médio
- **O que falta**: `CompetencyLibraryService.ListLibrariesAsync` retorna registros do banco, mas não há seed nem tool para criar bibliotecas. Instâncias novas sempre recebem lista vazia, impossibilitando avaliações EC baseadas em competências.
- **Onde**: `src/Eleva.Services/Services/Assessments/CompetencyService.cs`
- **Sugestão**: Criar seed de bibliotecas padrão (ex: liderança, técnicas, comportamentais) ou adicionar `competency_library_create` ao `EcMcpService`.

---

## Tabela de Prioridades

| # | Lacuna | Módulo | Impacto | Esforço |
|---|--------|--------|---------|---------|
| 16 | `review_360_create` sempre passa `reviewerId = 0` | Performance | Crítico | Baixo |
| 18 | BSC sem seed de perspectivas — inutilizável | BSC | Alto | Baixo |
| 1 | `employee_create` com status `Terminated` por padrão | People | Alto | Baixo |
| 21 | `InstanceId` não atribuído (multi-tenant quebrado) | Transversal | Alto | Médio |
| 17 | `OverallScore` nunca calculado após submit de review | Performance | Alto | Baixo |
| 9 | `MaturityLevel` nunca calculado no EC | EC | Alto | Médio |
| 10 | `GetOrgMaturityAsync` sempre retorna 0 | EC | Alto | Baixo* |
| 5 | Cálculo DISC ignora `AnswerValue` | DISC | Alto | Médio |
| 6 | Campos interpretativos DISC sempre null | DISC | Alto | Alto |
| 11 | Campos analíticos EC sempre null | EC | Alto | Alto |
| 13 | `Progress` PDI nunca calculado | PDI | Alto | Médio |
| 14 | Tools de atualização PDI ausentes | PDI | Alto | Médio |
| 22 | Auditoria completamente ausente | Transversal | Alto | Alto |
| 24 | Módulos Engagement/AI/Questions/Auth não implementados | Transversal | Alto | Muito Alto |
| 19 | `CalibrationScore`, `ClosedAt`, `BscGoalPO.Progress` nunca definidos | Performance | Médio | Médio |
| 4 | `assessmentType` descartado silenciosamente | DISC | Alto | Baixo |
| 8 | `disc_assessment_list` ausente | DISC | Médio | Baixo |
| 12 | `ec_assessment_list` ausente | EC | Médio | Baixo |
| 20 | `review_list` ausente | Performance | Médio | Baixo |
| 2 | `department_get` ausente no MCP | People | Médio | Baixo |
| 3 | Cargos/times somente leitura no MCP | People | Médio | Médio |
| 7 | `StartedAt`/`AssessmentLink`/`ReportUrl` DISC nunca definidos | DISC | Médio | Médio |
| 15 | `pdi_evidence_upload` sem validação | PDI | Médio | Baixo |
| 23 | Jobs Quartz não implementados | Transversal | Médio | Alto |
| 25 | `CompetencyLibraryPO` sem seed | EC | Médio | Baixo |

*Lacuna #10 se resolve automaticamente ao corrigir a #9.
