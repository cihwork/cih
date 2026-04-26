# Lacunas Funcionais — Engagement, AI Copilot, Questions, Auth, Infra

> Análise baseada em leitura direta do código-fonte. Cada lacuna cita arquivo e método real.
> Data: 2026-04-26

---

## Resumo Executivo

O Eleva possui scaffolding funcional completo (POs, interfaces, MCP tools registradas, jobs Quartz), mas a camada de negócio está majoritariamente não implementada. Os 6 jobs Quartz são placeholders puros (só fazem `LogInformation`), os 4 MCPs de Auth lançam `NotImplementedException`, o AI Copilot não chama nenhuma API de IA real (apenas lê recomendações já existentes no banco), e há bugs lógicos concretos em `mood_team_summary` e em `ConfigService.SetAsync`. O sistema persiste e lista dados corretamente, mas não produz inteligência, não notifica, não audita automaticamente e não tem mecanismo de autenticação funcional.

---

## Módulo: Engagement (Pulse / Feedback / CheckIn)

### 1. Pulse Survey sem mecanismo de envio ou agendamento

- **Impacto**: Alto
- **O que falta**: `PulseService.CreateAsync` apenas persiste o survey no banco. Não há disparo de notificação, link de resposta, nem mecanismo de envio para participantes. `PulseSurveyJob` deveria complementar isso mas é placeholder puro.
- **Onde**: `PulseService.cs:CreateAsync`, `Jobs/PulseSurveyJob.cs:Execute`
- **Sugestão**: `PulseSurveyJob` deve consultar surveys com `Status = Active && StartDate <= today` e disparar convite/notificação para os empregados alvo. `CreateAsync` pode aceitar lista de destinatários e criar registros de convite pendentes.

### 2. PulseService.RespondAsync sem validação de resposta duplicada

- **Impacto**: Alto
- **O que falta**: Não há verificação se `EmployeeId` já respondeu ao mesmo `surveyId`. Qualquer chamada adicional persiste nova resposta, contaminando as médias de `GetResultsAsync`.
- **Onde**: `PulseService.cs:RespondAsync` (sem query de duplicata antes do `AddAsync`)
- **Sugestão**: Verificar existência de `PulseResponsePO` com mesmo `PulseSurveyId + EmployeeId` antes de inserir; retornar a resposta existente ou lançar erro de negócio.

### 3. GetResultsAsync não agrega AnswersJson

- **Impacto**: Médio
- **O que falta**: O resultado retorna `AverageEnergyLevel` e `SentimentDistribution`, mas o campo `AnswersJson` de cada `PulseResponsePO` nunca é desserializado ou agregado. As respostas às perguntas reais do survey são descartadas na análise.
- **Onde**: `PulseService.cs:GetResultsAsync`
- **Sugestão**: Desserializar `AnswersJson` por resposta, agrupar por `PulseQuestionId` e calcular distribuição/média por pergunta.

### 4. Duplicidade entre PulseSurveyPO.QuestionsJson e PulseQuestionPO

- **Impacto**: Médio
- **O que falta**: `PulseSurveyPO` tem campo `QuestionsJson` (string) E existe a entidade separada `PulseQuestionPO`. Quando `CreateAsync` persiste as perguntas via `PulseQuestionPO`, o `QuestionsJson` original do survey fica com o valor que veio do input, criando inconsistência entre as duas representações. Nenhum código sincroniza os dois.
- **Onde**: `PulseService.cs:CreateAsync`, `PersistenceObjects/Engagement/PulseSurveyPO.cs`
- **Sugestão**: Escolher uma representação canônica (preferencialmente `PulseQuestionPO`) e remover `QuestionsJson` do PO, ou sincronizá-los no `CreateAsync`.

### 5. mood_team_summary com bug de filtro

- **Impacto**: Alto
- **O que falta**: O handler da tool `mood_team_summary` em `EngagementMcpService.cs` filtra com `Where(x => x.EmployeeId == managerId || x.EmployeeId == managerId)` — ambos os lados usam `managerId`, nunca filtrando membros da equipe. O resultado retorna entradas de humor do próprio manager (ou de qualquer funcionário com Id igual), não do time.
- **Onde**: `EngagementMcpService.cs` — handler de `mood_team_summary`
- **Sugestão**: Buscar `TeamMemberPO` onde `TeamId` pertence ao manager, extrair `EmployeeId`s, e então filtrar `MoodEntries` por esses ids.

### 6. CheckIn.SupportNeeded sem alerta ou integração

- **Impacto**: Médio
- **O que falta**: `CheckInPO.SupportNeeded` é persistido mas nenhum serviço ou job reage a `SupportNeeded = true`. O manager nunca é notificado.
- **Onde**: `CheckInService.cs:CreateAsync`
- **Sugestão**: Após persistir, se `SupportNeeded == true`, registrar evento de alerta (notificação, audit log, ou item de atenção para o manager via `ManagerId`).

### 7. FeedbackService.CreateAsync sem validações de negócio

- **Impacto**: Médio
- **O que falta**: Não valida se `FromEmployeeId != ToEmployeeId` (auto-feedback), não verifica existência dos employees, não define `Status` inicial (`FeedbackRecordStatus`), e `DeliveredDate` nunca é preenchido automaticamente.
- **Onde**: `FeedbackService.cs:CreateAsync`
- **Sugestão**: Adicionar guarda para `from == to`, definir `Status = Pending` na criação, e setar `DeliveredDate = UtcNow` quando `Visibility = Immediate`.

---

## Módulo: AI Copilot

### 8. AiRecommendationService não chama nenhuma IA real

- **Impacto**: Alto
- **O que falta**: Todos os métodos (`RecommendPdiAsync`, `DiscInsightsAsync`, `TeamAnalysisAsync`, `LeadershipTipsAsync`, `QuestionSuggestAsync`) apenas consultam `AiRecommendationPO` já existentes no banco. Não há chamada a nenhuma API de IA (OpenAI, Anthropic, etc). Se o banco não tiver recomendações pré-populadas, todos os métodos retornam listas vazias.
- **Onde**: `AiRecommendationService.cs` — todos os métodos
- **Sugestão**: Implementar geração real: buscar contexto (PDI, DISC, Pulse), montar prompt, chamar API de LLM, persistir resultado em `AiRecommendationPO` e `AiInteractionPO`. Os campos `ModelUsed`, `TokensIn`, `TokensOut`, `ConfidenceScore` do PO foram projetados exatamente para isso.

### 9. AiInteractionPO nunca é criado

- **Impacto**: Médio
- **O que falta**: `AiInteractionPO` existe com campos `Prompt`, `PromptHash`, `ResponseSummary`, `ModelUsed`, `TokensIn`, `TokensOut` — mas nenhum service insere registros nessa tabela. Não há rastreabilidade das interações com IA.
- **Onde**: `AiRecommendationService.cs` — nenhum método chama `_db.AiInteractions.AddAsync`
- **Sugestão**: A cada geração de recomendação, criar `AiInteractionPO` com o prompt usado, hash, resposta resumida e metadados de custo.

### 10. QuestionSuggestAsync ignora employeeId e contextId

- **Impacto**: Médio
- **O que falta**: O método retorna as primeiras 20 perguntas ativas por `SortOrder`, ignorando completamente `employeeId` e `contextId`. Não há personalização.
- **Onde**: `AiRecommendationService.cs:QuestionSuggestAsync`
- **Sugestão**: Usar `employeeId` para verificar histórico de respostas e excluir perguntas já respondidas; usar `contextId` para filtrar banco de perguntas relevante ao contexto.

### 11. AiRecommendationJob não gera recomendações

- **Impacto**: Alto
- **O que falta**: `AiRecommendationJob.Execute` apenas faz `LogInformation`. Deveria ser o mecanismo que gera recomendações periodicamente para employees com PDI/DISC/CheckIn recentes.
- **Onde**: `Jobs/AiRecommendationJob.cs:Execute`
- **Sugestão**: Iterar sobre employees com dados recentes (últimos N dias), chamar `IAiRecommendationService` para cada um e persistir as recomendações geradas.

---

## Módulo: Questions

### 12. SelectSmartAsync ignora contextType e contextId

- **Impacto**: Alto
- **O que falta**: O método recebe `AiContextType contextType` e `int? contextId` mas os ignora completamente. Retorna perguntas ordenadas por `SortOrder` sem considerar o contexto.
- **Onde**: `QuestionBankService.cs:SelectSmartAsync`
- **Sugestão**: Filtrar bancos pelo `AiContextType` (ex: banco de PDI para `AiContextType.Pdi`) e usar `contextId` para personalização adicional.

### 13. ValidateCrossAsync com lógica insuficiente

- **Impacto**: Baixo
- **O que falta**: Apenas verifica se a pergunta existe e se `QuestionBankId == targetBankId`. O nome do método sugere validação cross-bank (compatibilidade entre bancos), mas a implementação só checa membership. Não verifica duplicatas semânticas, incompatibilidade de `QuestionType`/`AnswerFormat`, ou regras de reuso.
- **Onde**: `QuestionBankService.cs:ValidateCrossAsync`
- **Sugestão**: Implementar verificação de perguntas com texto similar no banco alvo (ou via hash), e validar compatibilidade de formato entre bancos.

### 14. AnswerPO nunca é lido ou usado em nenhum serviço

- **Impacto**: Médio
- **O que falta**: `AnswerPO` existe com campos `TextValue`, `NumericValue`, `BooleanValue`, `Score`, mas nenhum serviço cria ou consulta respostas nessa tabela. Parece que `AnswersJson` em outros POs (PulseResponsePO, EcResponsePO) foi usado como alternativa, tornando `AnswerPO` letra morta.
- **Onde**: `PersistenceObjects/Questions/AnswerPO.cs` — sem referências em services
- **Sugestão**: Definir explicitamente onde `AnswerPO` é usado (ex: respostas de `ReviewResponse`, check-ins estruturados) ou remover o PO se `AnswersJson` for a estratégia adotada.

---

## Módulo: Auth / Instâncias

### 15. Todos os MCPs de Auth lançam NotImplementedException

- **Impacto**: Crítico
- **O que falta**: `AuthMcpService` registra 4 tools (`auth_login`, `auth_refresh`, `instance_create`, `instance_list`), todas com `Handler = (_, _) => Task.FromException<object?>(new NotImplementedException())`. Não há nenhum `IAuthService` ou `IInstanceService` no projeto.
- **Onde**: `AuthMcpService.cs` — todos os handlers
- **Sugestão**: Implementar `IAuthService` com login por email/senha (verificando `UserPO.PasswordHash`), geração de JWT/token, e `IInstanceService` para criação de `InstancePO` com slug único.

### 16. UserPO.PasswordHash sem serviço de hashing

- **Impacto**: Crítico
- **O que falta**: `UserPO` tem campo `PasswordHash` mas não existe nenhum serviço que faça hash de senhas ou valide credenciais. Qualquer dado inserido diretamente no banco precisaria de hash manual.
- **Onde**: `PersistenceObjects/Core/UserPO.cs` — campo `PasswordHash`
- **Sugestão**: Implementar helper de hashing (BCrypt/Argon2) referenciado pelo `IAuthService`.

### 17. InstancePO sem validação de slug único

- **Impacto**: Alto
- **O que falta**: Não existe service de instância. Quando implementado, `instance_create` deve garantir unicidade do `Slug` antes de persistir.
- **Onde**: Não existe `IInstanceService`
- **Sugestão**: Criar `IInstanceService.CreateAsync` com verificação de unicidade de slug + geração de `McpApiKey`.

---

## Módulo: Infra (Audit, Config, ExecutionStatus)

### 18. AuditLogService sem método de escrita

- **Impacto**: Alto
- **O que falta**: `IAuditLogService` expõe apenas `ListAsync` e `SearchAsync`. Não há `WriteAsync` ou `CreateAsync`. Nenhum service do sistema chama auditoria ao criar/alterar entidades. A tabela `AuditLogs` ficará vazia em produção.
- **Onde**: `IAuditLogService.cs` e `AuditLogService.cs` — ausência de método de escrita
- **Sugestão**: Adicionar `Task WriteAsync(int instanceId, AuditLogPO log)` à interface; chamar nos services de domínio (ou via middleware/interceptor de EF Core) nos eventos de Create/Update/Delete.

### 19. ChangeLogPO nunca é populado

- **Impacto**: Médio
- **O que falta**: `ChangeLogPO` tem campos `BeforeJson`, `AfterJson`, `CorrelationId`, `Source`, projetados para rastrear diffs entre versões de entidades. Nenhum service ou interceptor preenche essa tabela.
- **Onde**: `PersistenceObjects/History/ChangeLogPO.cs` — sem referências em services
- **Sugestão**: Implementar interceptor de `DbContext` (`SaveChangesInterceptor`) que captura estado antes/depois e persiste em `ChangeLogPO`.

### 20. ConfigService.SetAsync não define InstanceId no novo ConfigurationPO

- **Impacto**: Alto
- **O que falta**: Ao criar nova configuração, o objeto `ConfigurationPO` é instanciado sem atribuir `InstanceId`. O registro será salvo com `InstanceId = 0` (default int), quebrando o isolamento multi-tenant.
- **Onde**: `ConfigService.cs:SetAsync` — bloco `if (config is null)`
- **Sugestão**: Adicionar `InstanceId = instanceId` na inicialização do novo `ConfigurationPO`.

### 21. ConfigurationPO.IsSecret/IsEncrypted nunca tratados

- **Impacto**: Médio
- **O que falta**: `ConfigurationPO` tem campos `IsSecret` e `IsEncrypted`, mas `ConfigService.SetAsync` e `GetAsync` não aplicam nenhuma encriptação ou mascaramento. Valores sensíveis são armazenados e retornados em texto claro.
- **Onde**: `ConfigService.cs:SetAsync` e `GetAsync`, `PersistenceObjects/Core/ConfigurationPO.cs`
- **Sugestão**: Quando `IsEncrypted = true`, encriptar o `Value` antes de persistir e decriptar no `GetAsync`. Quando `IsSecret = true`, mascarar o retorno no `ListAsync`.

### 22. ExecutionStatusService em memória perde dados no restart

- **Impacto**: Médio
- **O que falta**: `ExecutionStatusService` usa `private static readonly List<ExecutionStatusEntry> _entries` em memória. A spec menciona `RedisExecutionStatusStore` mas não foi implementado. Todos os registros de status são perdidos ao reiniciar o processo.
- **Onde**: `ExecutionStatusService.cs` — campo `_entries`
- **Sugestão**: Implementar `IExecutionStatusStore` com backing em Redis ou banco de dados, conforme previsto na spec (seção 4.4).

### 23. ICurrentInstanceAccessor injetado mas não usado nos services

- **Impacto**: Baixo
- **O que falta**: Todos os services (`PulseService`, `FeedbackService`, `CheckInService`, etc.) injetam `ICurrentInstanceAccessor _instanceAccessor` mas nunca o utilizam — o `instanceId` é sempre recebido como parâmetro explícito. O campo existe mas é letra morta.
- **Onde**: Todos os services em `src/Eleva.Services/Services/`
- **Sugestão**: Ou usar `_instanceAccessor.InstanceId` internamente (removendo o parâmetro explícito), ou remover a dependência dos construtores.

---

## Jobs Quartz

Todos os 6 jobs seguem o mesmo padrão placeholder — constroem apenas com `ILogger`, sem injetar nenhum service de domínio, e executam apenas `LogInformation`:

```csharp
public Task Execute(IJobExecutionContext context)
{
    _logger.LogInformation("{Job} executed at {Time}", nameof(XxxJob), DateTimeOffset.UtcNow);
    return Task.CompletedTask;
}
```

### PdiNotificationJob
- **O que deveria fazer**: Identificar PDI goals/actions com deadline próximo (`DueDate <= today + N dias`), notificar employee e manager, registrar checkpoint pendente.
- **O que falta**: Injetar `IPdiService`, consultar `PdiGoalPO` e `PdiActionPO`, disparar notificação.

### AssessmentCycleJob
- **O que deveria fazer**: Verificar ciclos de assessment em andamento, lembrar participantes de DISC ou EC pendentes, fechar ciclos expirados.
- **O que falta**: Injetar `IEcService`/`IDiscService`, verificar `AssessmentStatus`, transicionar estados.

### PulseSurveyJob
- **O que deveria fazer**: Ativar surveys com `StartDate <= today && Status = Draft`, enviar convites, fechar surveys com `EndDate < today`.
- **O que falta**: Injetar `IPulseService`, consultar `PulseSurveyPO` por status/data, transicionar status.

### PerformanceCycleJob
- **O que deveria fazer**: Avançar ciclos de performance (`PerformanceCyclePO`), notificar reviews pendentes, calcular progresso de `BscGoalPO`.
- **O que falta**: Injetar `IReviewService`/`IBscService`, processar ciclos ativos.

### AiRecommendationJob
- **O que deveria fazer**: Gerar recomendações de IA para employees com atividade recente (novos PDIs, CheckIns, resultados DISC).
- **O que falta**: Injetar `IAiRecommendationService`, iterar employees elegíveis, chamar métodos de recomendação, persistir.

### AuditConsolidationJob
- **O que deveria fazer**: Consolidar logs de auditoria, arquivar entradas antigas, gerar sumário periódico.
- **O que falta**: Injetar `IAuditLogService`, processar e consolidar `AuditLogPO` e `ChangeLogPO`.

---

## Tabela de Prioridades

| # | Lacuna | Módulo | Impacto | Esforço |
|---|--------|--------|---------|---------|
| 15 | Auth MCPs lançam NotImplementedException | Auth | Crítico | Alto |
| 16 | UserPO.PasswordHash sem serviço de hashing | Auth | Crítico | Médio |
| 8  | AiRecommendationService não chama IA real | AI Copilot | Alto | Alto |
| 20 | ConfigService.SetAsync não define InstanceId | Infra | Alto | Baixo |
| 18 | AuditLogService sem método de escrita | Infra | Alto | Médio |
| 5  | mood_team_summary com bug de filtro | Engagement | Alto | Baixo |
| 1  | Pulse Survey sem envio/notificação | Engagement | Alto | Alto |
| 2  | PulseService sem validação de resposta duplicada | Engagement | Alto | Baixo |
| 17 | InstancePO sem validação de slug único | Auth | Alto | Baixo |
| 11 | AiRecommendationJob não gera recomendações | Jobs | Alto | Alto |
| 25 | PulseSurveyJob placeholder puro | Jobs | Alto | Médio |
| 22 | ExecutionStatusService perde dados no restart | Infra | Médio | Médio |
| 12 | SelectSmartAsync ignora contextType e contextId | Questions | Alto | Médio |
| 9  | AiInteractionPO nunca é criado | AI Copilot | Médio | Médio |
| 10 | QuestionSuggestAsync ignora employeeId e contextId | AI Copilot | Médio | Médio |
| 3  | GetResultsAsync não agrega AnswersJson | Engagement | Médio | Médio |
| 6  | CheckIn.SupportNeeded sem alerta | Engagement | Médio | Baixo |
| 7  | FeedbackService sem validações de negócio | Engagement | Médio | Baixo |
| 19 | ChangeLogPO nunca é populado | Infra | Médio | Alto |
| 21 | ConfigurationPO.IsSecret/IsEncrypted ignorados | Infra | Médio | Médio |
| 4  | Duplicidade PulseSurveyPO.QuestionsJson vs PulseQuestionPO | Engagement | Médio | Médio |
| 14 | AnswerPO nunca usado | Questions | Médio | Baixo |
| 13 | ValidateCrossAsync com lógica insuficiente | Questions | Baixo | Médio |
| 23 | ICurrentInstanceAccessor injetado mas não usado | Infra | Baixo | Baixo |
