# Governança, Compliance LGPD e Segurança - MCP-RH

**Versão:** 1.0
**Data:** 2026-04-13
**Status:** Draft
**Classificação:** Confidencial

---

## Sumário Executivo

Este documento estabelece o framework de governança, compliance com a Lei Geral de Proteção de Dados (LGPD - Lei 13.709/2018) e controles de segurança da informação para o MCP-RH.

O MCP-RH processa dados pessoais e sensíveis de colaboradores, incluindo avaliações comportamentais (DISC), desempenho, clima organizacional e planos de desenvolvimento individual (PDI). Por sua natureza, o sistema exige controles rigorosos de privacidade, segurança e governança.

**Princípios Fundamentais:**
- **Privacy by Design**: Proteção de dados desde a concepção
- **Data Minimization**: Coleta apenas do necessário
- **Transparency**: Clareza sobre uso e finalidade
- **Accountability**: Responsabilidade demonstrável
- **Security First**: Segurança em todas as camadas

---

## 1. Compliance LGPD

### 1.1 Mapeamento de Dados Pessoais

#### 1.1.1 Inventário de Dados

| Categoria | Tipo de Dado | Classificação | Base Legal | Retenção | Controlador |
|-----------|--------------|---------------|------------|----------|-------------|
| **Identificação** | Nome completo, CPF, RG, e-mail, telefone | Pessoal | Execução de contrato | Duração do vínculo + 5 anos | Tenant (empresa cliente) |
| **Profissional** | Cargo, departamento, liderança, data admissão | Pessoal | Execução de contrato | Duração do vínculo + 5 anos | Tenant |
| **Comportamental (DISC)** | Perfil DISC, análise comportamental | **Sensível** | Consentimento explícito | Até revogação ou término + 2 anos | Tenant |
| **Desempenho** | Metas BSC, indicadores, avaliações | Pessoal | Legítimo interesse | Duração do vínculo + 5 anos | Tenant |
| **PDI** | Gaps de competência, ações de desenvolvimento | Pessoal | Legítimo interesse | Duração do vínculo + 5 anos | Tenant |
| **Clima e Pulso** | Respostas de pesquisa, humor, energia | **Sensível** | Consentimento explícito | Até revogação ou término + 2 anos | Tenant |
| **Check-ins e 1:1** | Anotações de reuniões, feedback | Pessoal | Legítimo interesse | Duração do vínculo + 3 anos | Tenant |
| **Acesso e Auditoria** | Logs de acesso, IPs, ações no sistema | Pessoal | Obrigação legal | 6 meses (mínimo LGPD) | MCP-RH (Operador) |

#### 1.1.2 Dados Sensíveis - Controles Especiais

**Dados comportamentais e psicológicos (DISC, EC) exigem:**
- ✅ Consentimento livre, informado e inequívoco
- ✅ Opt-in explícito (checkbox separado, não pré-marcado)
- ✅ Finalidade específica e clara
- ✅ Possibilidade de revogação a qualquer momento
- ✅ Segregação lógica e física de armazenamento
- ✅ Criptografia em repouso (AES-256)
- ✅ Mascaramento em logs e relatórios gerenciais
- ✅ Acesso restrito por RBAC com aprovação adicional

**Dados de clima e pulso emocional:**
- ✅ Anonimização quando agregados
- ✅ Consentimento separado para uso identificado
- ✅ Impossibilidade de cruzamento reverso (re-identificação)
- ✅ Logs de acesso rastreáveis

### 1.2 Base Legal para Tratamento

#### 1.2.1 Matriz de Base Legal

| Finalidade | Base Legal (Art. 7º/11º LGPD) | Justificativa |
|------------|-------------------------------|---------------|
| Gestão de PDI e desenvolvimento | Execução de contrato (Art. 7º, V) | Necessário para cumprimento da relação de trabalho |
| Avaliação de desempenho BSC | Legítimo interesse (Art. 7º, IX) | Interesse legítimo da empresa em gerenciar resultados |
| Perfil comportamental DISC | Consentimento (Art. 11º, I) | Dado sensível - requer consentimento específico |
| Pesquisa de clima e pulso | Consentimento (Art. 11º, I) | Dado sensível - opinião e estado emocional |
| Auditoria e compliance | Obrigação legal (Art. 7º, II) | Cumprimento de obrigação regulatória |
| Recomendações de IA | Legítimo interesse (Art. 7º, IX) | Melhoria da gestão de pessoas |

#### 1.2.2 Documentação de Legítimo Interesse (LIA - Legitimate Interest Assessment)

**Para Avaliação de Desempenho:**
- **Finalidade**: Acompanhar metas e resultados alinhados ao BSC
- **Necessidade**: Gestão eficaz de equipes e estratégia
- **Balanceamento**: Interesse da empresa vs. expectativa razoável do colaborador
- **Salvaguardas**: Acesso restrito, uso apenas para gestão, não para decisões automatizadas punitivas

**Para Recomendações de IA:**
- **Finalidade**: Apoiar BP, coach e líder com insights contextuais
- **Necessidade**: Personalização do acompanhamento e sugestões de ação
- **Balanceamento**: Benefício para desenvolvimento do colaborador vs. sensibilidade dos dados
- **Salvaguardas**: IA como copiloto (não decisor), explicabilidade, auditoria de prompts

### 1.3 Direitos dos Titulares

#### 1.3.1 Catálogo de Direitos (Art. 18 LGPD)

| Direito | Como Exercer | Prazo Resposta | Responsável |
|---------|--------------|----------------|-------------|
| **Confirmação e acesso** | Formulário web ou e-mail ao DPO | 15 dias | DPO + BP/RH do tenant |
| **Correção de dados** | Solicitação via sistema ou DPO | 5 dias úteis | Administrador do tenant |
| **Anonimização/bloqueio/eliminação** | Solicitação formal ao DPO | 15 dias | DPO + Arquitetura |
| **Portabilidade** | Export em formato estruturado (JSON/CSV) | 15 dias | Equipe técnica |
| **Informação sobre compartilhamento** | Relatório de integrações ativas | 15 dias | DPO |
| **Revogação de consentimento** | Botão no perfil ou e-mail ao DPO | Imediato | Sistema automatizado |
| **Oposição ao tratamento** | Formulário específico | 15 dias | DPO + Jurídico |

#### 1.3.2 Workflow de Atendimento

```
Solicitação do titular
    ↓
Validação de identidade (2FA ou documento)
    ↓
Análise de viabilidade (DPO)
    ↓
Execução técnica (se procedente)
    ↓
Notificação ao titular (com protocolo)
    ↓
Registro em log de auditoria LGPD
```

**Exceções legais (Art. 18, §§):**
- Cumprimento de obrigação legal
- Exercício regular de direitos em processo
- Proteção da vida ou incolumidade física

### 1.4 Processos de Consentimento

#### 1.4.1 Fluxo de Opt-in para Dados Sensíveis

**Momento: Onboarding ou ativação de módulo**

```
┌─────────────────────────────────────────────┐
│  Bem-vindo ao MCP-RH!                       │
│                                             │
│  Para personalizar sua experiência,         │
│  precisamos do seu consentimento para:      │
│                                             │
│  [ ] Realizar avaliação DISC                │
│      Finalidade: Entender seu perfil        │
│      comportamental para recomendações      │
│      de comunicação e liderança.            │
│      Você pode revogar a qualquer momento.  │
│                                             │
│  [ ] Participar de pesquisas de clima       │
│      Finalidade: Medir engajamento e        │
│      energia do time. Dados agregados       │
│      são anonimizados.                      │
│                                             │
│  [Ler Política de Privacidade completa]     │
│  [Confirmar] [Recusar]                      │
└─────────────────────────────────────────────┘
```

**Registro de consentimento:**
- ✅ Timestamp UTC
- ✅ IP de origem
- ✅ Versão do termo aceito
- ✅ Finalidade específica
- ✅ Forma de obtenção (web, API, import)

#### 1.4.2 Gestão de Revogação

**Onde revogar:**
- Painel do colaborador: "Minhas Preferências de Privacidade"
- E-mail ao DPO: dpo@[cliente].com.br

**Efeitos da revogação:**
- ✅ Bloqueio imediato de novos tratamentos
- ✅ Mascaramento de dados existentes em relatórios
- ✅ Exclusão após período legal (se aplicável)
- ✅ Notificação ao BP/RH do tenant
- ✅ Logs preservados para compliance

### 1.5 DPO e Responsabilidades

#### 1.5.1 Papéis LGPD

| Papel | Entidade | Responsabilidade |
|-------|----------|------------------|
| **Controlador** | Tenant (empresa cliente) | Define finalidades e meios de tratamento |
| **Operador** | MCP-RH (plataforma SaaS) | Trata dados conforme instruções do controlador |
| **DPO do Controlador** | Indicado pelo tenant | Canal com titulares, orienta RH do cliente |
| **DPO do Operador** | Indicado pelo MCP-RH | Compliance da plataforma, incidentes, ANPD |
| **Encarregado Técnico** | Equipe de Arquitetura | Implementa controles, anonimização, segurança |

#### 1.5.2 Atribuições do DPO do Operador (MCP-RH)

- Manter registro de operações de tratamento (ROPA)
- Orientar colaboradores sobre boas práticas LGPD
- Atender requisições da ANPD
- Coordenar resposta a incidentes de dados
- Revisar contratos de processamento (DPA)
- Conduzir DPIA (Data Protection Impact Assessment)
- Treinar equipes de desenvolvimento e suporte

**Contato DPO MCP-RH:**
📧 dpo@mcprh.com.br
📱 Canal Seguro (criptografado)

---

## 2. Auditoria e Rastreabilidade

### 2.1 Logs de Acesso a Dados Sensíveis

#### 2.1.1 Eventos Auditados

| Evento | Nível | Dados Capturados | Retenção |
|--------|-------|------------------|----------|
| Login e autenticação | INFO | User ID, IP, timestamp, método (SSO, senha, 2FA) | 6 meses |
| Acesso a perfil DISC | AUDIT | User ID, target ID, ação, timestamp, tenant | 2 anos |
| Visualização de PDI de terceiros | AUDIT | User ID, target ID, contexto (1:1, BP), tenant | 2 anos |
| Modificação de dados pessoais | AUDIT | User ID, campo alterado, valor antigo (hash), novo (hash) | 5 anos |
| Export de dados (portabilidade) | CRITICAL | User ID, tipo de export, registros incluídos | 5 anos |
| Exclusão de dados (LGPD) | CRITICAL | User ID, tipo de exclusão, base legal, protocolo | Permanente |
| Consulta de IA (recomendações) | AUDIT | Prompt (anonimizado), resposta (resumo), contexto | 1 ano |
| Acesso a pesquisa de clima | AUDIT | User ID, filtros aplicados, nível de agregação | 2 anos |
| Tentativa de acesso negado | WARNING | User ID, recurso, razão da negação | 1 ano |

#### 2.1.2 Formato de Log

**Padrão JSON estruturado:**

```json
{
  "timestamp": "2026-04-13T14:32:18.123Z",
  "event_type": "DATA_ACCESS",
  "severity": "AUDIT",
  "tenant_id": "tenant_abc123",
  "user_id": "user_xyz789",
  "user_role": "BP",
  "action": "VIEW_DISC_PROFILE",
  "target_user_id": "user_def456",
  "ip_address": "192.168.1.100",
  "user_agent": "Mozilla/5.0...",
  "session_id": "sess_abcdef123456",
  "context": {
    "module": "Assessments",
    "justification": "1:1 preparation",
    "consent_valid": true
  },
  "result": "SUCCESS",
  "data_classification": "SENSITIVE",
  "retention_days": 730
}
```

### 2.2 Trilha de Auditoria

#### 2.2.1 Camadas de Auditoria

**Camada 1: Application Layer**
- Logs de negócio (quem fez o quê)
- Rastreamento de mudanças (audit trail)
- Correlação de eventos por sessão

**Camada 2: Database Layer**
- Triggers de auditoria em tabelas sensíveis
- Change Data Capture (CDC) para histórico
- Row-level security logs

**Camada 3: Infrastructure Layer**
- Logs de acesso a servidores (SSH, RDP)
- Logs de rede (firewall, IDS/IPS)
- Logs de container/orquestração (Kubernetes)

**Camada 4: Platform Layer**
- Azure/AWS CloudTrail
- Azure AD / Entra ID audit logs
- API Gateway access logs

#### 2.2.2 Correlação e SIEM

**Integração com SIEM (Security Information and Event Management):**
- Centralização de logs em plataforma SIEM (ex: Splunk, Elastic, Azure Sentinel)
- Correlação de eventos multi-camada
- Alertas em tempo real para atividades suspeitas
- Dashboards de compliance e segurança

**Regras de correlação:**
- ⚠️ Acesso a >50 perfis DISC em <10 minutos → Alerta de exfiltração
- ⚠️ Export massivo de dados + login de IP não usual → Incident response
- ⚠️ Múltiplas tentativas de acesso negado → Possível ataque
- ⚠️ Modificação de consentimento + acesso imediato → Revisão manual

### 2.3 Retenção de Logs

#### 2.3.1 Política de Retenção

| Tipo de Log | Retenção Ativa | Retenção Arquivada | Justificativa |
|-------------|----------------|--------------------|-----------------|
| Acesso (geral) | 6 meses | 2 anos | LGPD Art. 37 + boas práticas |
| Dados sensíveis (DISC, clima) | 2 anos | 5 anos | Evidência de consentimento |
| Exclusão/Portabilidade | 5 anos | Permanente | Obrigação legal |
| Incidentes de segurança | 5 anos | Permanente | Regulação e defesa legal |
| Mudanças de configuração | 3 anos | 5 anos | Auditoria técnica |

**Arquivamento:**
- Formato WORM (Write Once Read Many)
- Armazenamento em cold storage (Azure Archive, AWS Glacier)
- Criptografia AES-256
- Hash SHA-256 para integridade

### 2.4 Relatórios de Auditoria

#### 2.4.1 Relatórios Programados

**Mensal (para DPO e CISO):**
- Total de acessos a dados sensíveis
- Requisições LGPD (tipo e status)
- Incidentes de segurança
- Violações de política de acesso
- Consentimentos concedidos/revogados

**Trimestral (para Governança):**
- Aderência a políticas de retenção
- Revisão de acessos privilegiados
- Status de pendências de auditoria
- Evolução de riscos

**Anual (para Auditoria Externa):**
- Relatório completo de compliance LGPD
- Evidências de controles SOC 2 / ISO 27001
- Testes de recuperação de desastres
- Penetration testing e remediações

#### 2.4.2 Relatórios sob Demanda

**Para requisições ANPD ou auditorias:**
- Extração de logs por período específico
- Relatório de tratamento de dados de um titular
- Evidência de destruição de dados
- Histórico de acessos a um registro específico

---

## 3. Segurança da Informação

### 3.1 Criptografia

#### 3.1.1 Criptografia em Trânsito

**Protocolos e Padrões:**
- ✅ TLS 1.3 (mínimo TLS 1.2) para HTTPS
- ✅ Certificados SSL/TLS emitidos por CA confiável (Let's Encrypt, DigiCert)
- ✅ HSTS (HTTP Strict Transport Security) habilitado
- ✅ Perfect Forward Secrecy (PFS) com ECDHE
- ✅ Cipher suites fortes (AES-256-GCM, ChaCha20-Poly1305)
- ❌ SSLv3, TLS 1.0, TLS 1.1 desabilitados
- ❌ Cipher suites fracos (RC4, DES, 3DES) bloqueados

**APIs e Integrações:**
- ✅ Mutual TLS (mTLS) para integrações MCP críticas
- ✅ Validação de certificado client-side
- ✅ Pinning de certificado para apps mobile

#### 3.1.2 Criptografia em Repouso

**Banco de Dados:**
- ✅ Transparent Data Encryption (TDE) habilitado
- ✅ Criptografia nativa do SQL Server / PostgreSQL
- ✅ Criptografia de coluna para dados sensíveis (DISC, clima)
  - Algoritmo: AES-256-CBC
  - Key management: Azure Key Vault / AWS KMS
  - Rotation: Anual ou sob demanda
- ✅ Backup criptografado (AES-256)

**Armazenamento de Arquivos:**
- ✅ Azure Blob Storage / AWS S3 com Server-Side Encryption (SSE)
- ✅ Criptografia client-side para documentos sensíveis
- ✅ Versionamento habilitado com criptografia por versão

**Chaves de Criptografia:**
- ✅ Armazenamento em HSM (Hardware Security Module) ou cloud KMS
- ✅ Segregação por tenant (chave única por cliente)
- ✅ Rotação automática (anual)
- ✅ Auditoria de uso de chaves
- ❌ Nunca em código-fonte, variáveis de ambiente não seguras ou logs

### 3.2 Gerenciamento de Secrets

#### 3.2.1 Vault e KMS

**Solução:**
- Azure Key Vault (Azure)
- AWS Secrets Manager (AWS)
- HashiCorp Vault (on-premises/híbrido)

**Tipos de Secrets:**
- Strings de conexão de banco de dados
- API keys de integrações (MCP, WhatsApp, Teams)
- Certificados SSL/TLS
- Chaves de criptografia simétricas
- Tokens OAuth/OIDC

**Boas Práticas:**
- ✅ Acesso por Managed Identity (Azure) ou IAM Role (AWS)
- ✅ Versionamento de secrets
- ✅ Rotação automática (90 dias para API keys)
- ✅ Auditoria de acesso
- ✅ Princípio do menor privilégio
- ❌ Nunca em appsettings.json, .env commitado, ou código

#### 3.2.2 Rotação de Credenciais

**Frequência:**
- Senhas de serviço: 90 dias
- API keys: 180 dias
- Certificados: 1 ano (renovação automática 30 dias antes)
- Chaves de criptografia: 1 ano

**Processo:**
```
1. Gerar nova credencial em Vault
2. Atualizar aplicação (canary deployment)
3. Verificar funcionamento
4. Revogar credencial antiga
5. Registrar em log de auditoria
```

### 3.3 Controle de Acesso (RBAC)

#### 3.3.1 Papéis e Permissões

| Papel | Escopo | Permissões |
|-------|--------|------------|
| **Tenant Admin** | Tenant específico | Gerenciar usuários, configurar tenant, acessar todos os módulos, exportar dados |
| **BP / RH** | Tenant específico | Visualizar carteira de pessoas, acessar relatórios consolidados, ver DISC e PDI de time |
| **Coach / Consultor** | Cross-tenant (com autorização) | Criar e acompanhar PDIs, acessar DISC com consentimento, gerar diagnósticos |
| **Líder** | Time sob sua gestão | Ver PDI de liderados, receber alertas, fazer 1:1, dar feedback |
| **Colaborador** | Próprio perfil + compartilhado | Ver e editar próprio PDI, responder pesquisas, visualizar metas BSC |
| **Auditor** | Cross-tenant (read-only) | Acessar logs, relatórios de compliance, trilha de auditoria |
| **Platform Admin** | Sistema global | Gerenciar tenants, configurar integrações, acessar infra |

#### 3.3.2 Matriz de Acesso Granular

**Módulo: DISC (dados sensíveis)**

| Papel | Próprio | Liderados Diretos | Time Indireto | Qualquer Pessoa |
|-------|---------|-------------------|---------------|-----------------|
| Colaborador | RW | - | - | - |
| Líder | RW | R (com consentimento) | - | - |
| BP | R (com justificativa) | R | R (agregado) | R (agregado) |
| Coach | R (com consentimento) | R (com consentimento) | R (com consentimento) | - |
| Auditor | - | - | - | R (logs, não conteúdo) |

**R** = Read, **W** = Write, **-** = Sem acesso

#### 3.3.3 Implementação Técnica

**ASP.NET Core:**

```csharp
[Authorize(Roles = "BP,Coach")]
[RequireTenantContext]
[AuditLog(Sensitivity = DataClassification.Sensitive)]
public async Task<IActionResult> GetDISCProfile(Guid userId)
{
    // Valida consentimento
    if (!await _consentService.HasConsentAsync(userId, ConsentType.DISCProfile))
        return Forbid("Consentimento não concedido");

    // Valida escopo (líder pode ver apenas liderados)
    if (!await _authService.CanAccessUserData(CurrentUser, userId))
        return Forbid("Acesso negado por escopo");

    // Registra acesso em auditoria
    await _auditService.LogAccessAsync(new AuditEntry
    {
        Action = "VIEW_DISC_PROFILE",
        TargetUserId = userId,
        Justification = Request.Headers["X-Access-Justification"]
    });

    return Ok(await _discService.GetProfileAsync(userId));
}
```

**Atributos customizados:**
- `[RequireTenantContext]`: Isola dados por tenant
- `[RequireConsent(ConsentType)]`: Valida opt-in
- `[AuditLog]`: Registra acesso automaticamente
- `[RequireApproval]`: Acesso sob demanda com aprovação

### 3.4 Segregação Multi-Tenant

#### 3.4.1 Estratégia de Isolamento

**Modelo adotado: Pool de recursos com isolamento lógico**

**Camadas de segregação:**

1. **Banco de Dados:**
   - Schema por tenant (`tenant_abc123.People`, `tenant_xyz789.People`)
   - Row-level security (RLS) via `TenantId`
   - Índices filtrados por tenant
   - Backup segregado por tenant

2. **Armazenamento:**
   - Containers/Buckets separados (`storage/tenant_abc123/`)
   - Permissões IAM isoladas
   - Criptografia com chaves únicas

3. **Aplicação:**
   - Middleware de tenant resolution (por domínio, header, claim)
   - Query filter global (`_dbContext.TenantId == currentTenant`)
   - Cache particionado por tenant

4. **Logs e Auditoria:**
   - Índice por `tenant_id`
   - Segregação de acesso a logs (tenant admin vê apenas seu tenant)

#### 3.4.2 Prevenção de Cross-Tenant Leakage

**Controles:**
- ✅ Validação de tenant em TODA query (global query filter)
- ✅ Testes automatizados de isolamento (unit + integration)
- ✅ Revisão de código com checklist de tenant safety
- ✅ Penetration testing focado em bypass de tenant
- ✅ Sanitização de cache (chave inclui `tenant_id`)
- ✅ Validação dupla em APIs (JWT claim + database)

**Exemplo de teste:**

```csharp
[Fact]
public async Task Should_Not_Return_Data_From_Other_Tenant()
{
    // Arrange
    var tenant1User = CreateUser(tenantId: "tenant1");
    var tenant2User = CreateUser(tenantId: "tenant2");

    // Act
    var result = await _pdiService.GetPDIsAsync(tenant1User);

    // Assert
    Assert.DoesNotContain(result, pdi => pdi.UserId == tenant2User.Id);
}
```

---

## 4. Processos de Governança

### 4.1 Aprovações e Workflows

#### 4.1.1 Matriz de Aprovações

| Ação | Aprovador 1 | Aprovador 2 | Justificativa Obrigatória |
|------|-------------|-------------|---------------------------|
| Criar novo tenant | Platform Admin | Comercial | Sim (contrato) |
| Modificar configuração de retenção | DPO | Tenant Admin | Sim (justificativa legal) |
| Acesso emergencial a dados (break-glass) | CISO | DPO | Sim (incidente) |
| Export massivo de dados | Tenant Admin | DPO | Sim (finalidade) |
| Ativar módulo de dados sensíveis (DISC) | Tenant Admin | - | Sim (política interna) |
| Revogar acesso de usuário | Líder ou BP | Tenant Admin (se admin) | Não |
| Modificar papéis e permissões | Tenant Admin | - | Não |
| Exclusão permanente de dados (LGPD) | DPO | Jurídico | Sim (base legal) |

#### 4.1.2 Workflow de Aprovação - Exemplo: Export de Dados

```
1. Solicitação (Tenant Admin)
   ↓
2. Validação automática (sistema)
   - Verifica se solicitante tem papel adequado
   - Checa se há consentimento válido
   - Valida período e escopo
   ↓
3. Aprovação (DPO)
   - Revisa justificativa
   - Confirma conformidade LGPD
   - Aprova ou rejeita
   ↓
4. Execução (Sistema)
   - Gera export em formato seguro
   - Criptografa arquivo
   - Disponibiliza via link temporário (24h)
   ↓
5. Notificação e Log
   - Notifica solicitante
   - Registra em audit trail
   - Atualiza ROPA (Registro de Operações)
```

### 4.2 Gestão de Mudanças (Change Management)

#### 4.2.1 Classificação de Mudanças

| Tipo | Exemplos | Aprovação | Testes Obrigatórios |
|------|----------|-----------|---------------------|
| **Baixo Risco** | Texto de interface, correção de bug visual | Tech Lead | Unit tests |
| **Médio Risco** | Nova feature sem dados sensíveis, API não crítica | Product Owner + Tech Lead | Unit + Integration |
| **Alto Risco** | Mudança em RBAC, algoritmo de IA, migração de dados | CISO + DPO + Arquitetura | Unit + Integration + Security + UAT |
| **Crítico** | Mudança em criptografia, multi-tenant, autenticação | CISO + DPO + C-Level | Todos + Pentest |

#### 4.2.2 Processo de Change

**Fases:**

1. **Planejamento (RFC - Request for Change)**
   - Descrever mudança e impacto
   - Avaliar riscos (segurança, compliance, disponibilidade)
   - Definir rollback plan

2. **Aprovação**
   - Revisão por Change Advisory Board (CAB)
   - Validação de testes e evidências
   - Autorização formal

3. **Implementação**
   - Deploy em ambiente de staging
   - Testes de regressão e segurança
   - Deploy em produção (canary/blue-green)

4. **Validação**
   - Smoke tests pós-deploy
   - Monitoramento de logs e métricas
   - Rollback se necessário

5. **Documentação**
   - Atualizar runbooks
   - Registrar lições aprendidas
   - Atualizar ROPA se impactar dados

### 4.3 Revisão de Acessos (Access Review)

#### 4.3.1 Frequência de Revisão

| Tipo de Acesso | Frequência | Responsável | Ação se Não Revisado |
|----------------|------------|-------------|----------------------|
| Colaboradores ativos | Trimestral | BP + Tenant Admin | Alerta (manter acesso) |
| Acessos privilegiados (admin, DPO) | Mensal | CISO | Suspensão automática |
| Acessos de terceiros (coach, consultor) | Mensal | Tenant Admin | Suspensão automática |
| Acessos de auditores | Após auditoria | CISO | Revogação automática |
| Acessos emergenciais (break-glass) | Após uso | CISO + DPO | Revogação automática |

#### 4.3.2 Processo de Revisão

**Automatizado:**
- Sistema gera relatório de acessos por tenant
- Envia para responsável via e-mail
- Responsável confirma ou revoga acessos via interface
- Acessos não revisados geram alerta escalonado

**Manual (para acessos sensíveis):**
- Reunião mensal do CAB
- Revisão de justificativas de acesso
- Validação de least privilege
- Documentação de decisões

### 4.4 Gestão de Incidentes de Segurança

#### 4.4.1 Classificação de Incidentes

| Severidade | Exemplos | Tempo de Resposta | Notificação |
|------------|----------|-------------------|-------------|
| **SEV 1 (Crítico)** | Vazamento de dados sensíveis, ransomware, comprometimento de autenticação | Imediata (15 min) | CISO, DPO, CEO, ANPD (se aplicável) |
| **SEV 2 (Alto)** | Tentativa de intrusão, indisponibilidade de módulo crítico | 1 hora | CISO, DPO, Tenant Admin |
| **SEV 3 (Médio)** | Falha de segurança sem vazamento, violação de política | 4 horas | Equipe de Segurança |
| **SEV 4 (Baixo)** | Tentativa de acesso não autorizado bloqueada | 24 horas | Log automático |

#### 4.4.2 Fluxo de Resposta a Incidente

```
1. Detecção (SIEM, alerta, reporte)
   ↓
2. Triagem (15 min)
   - Classificar severidade
   - Acionar time de resposta
   ↓
3. Contenção (30 min - 4h)
   - Isolar sistema afetado
   - Bloquear acesso malicioso
   - Preservar evidências
   ↓
4. Investigação (1-7 dias)
   - Análise forense
   - Identificar causa raiz
   - Avaliar impacto (dados afetados, titulares)
   ↓
5. Erradicação e Recuperação
   - Remover ameaça
   - Restaurar serviços
   - Validar integridade
   ↓
6. Notificação (LGPD Art. 48)
   - ANPD: em até 72h (se alto risco)
   - Titulares: "em prazo razoável"
   - Tenants afetados: imediatamente
   ↓
7. Post-Mortem
   - Lições aprendidas
   - Atualizar runbooks
   - Implementar melhorias
```

#### 4.4.3 Notificação de Violação (LGPD)

**Critérios para notificação à ANPD (Art. 48):**
- Risco ou dano relevante aos titulares
- Dados sensíveis (DISC, clima) comprometidos
- Impacto a direitos e liberdades

**Conteúdo da notificação:**
- Descrição da violação
- Dados afetados (tipo e volume)
- Titulares afetados (estimativa)
- Medidas técnicas de proteção (criptografia)
- Riscos aos titulares
- Medidas adotadas para mitigar
- Contato do DPO

**Modelo de comunicação aos titulares:**

> Prezado(a) [Nome],
>
> Informamos que, em [data], identificamos um incidente de segurança que pode ter afetado seus dados pessoais no sistema MCP-RH.
>
> **Dados potencialmente afetados:** [tipo de dados]
> **Causa:** [breve descrição]
> **Medidas adotadas:** [ações de contenção e mitigação]
>
> Seus dados estavam protegidos por criptografia [AES-256], o que reduz significativamente o risco de uso indevido.
>
> **Recomendações:**
> - Altere sua senha imediatamente
> - Ative autenticação de dois fatores
> - Monitore atividades suspeitas
>
> Para dúvidas, entre em contato com nosso DPO: dpo@mcprh.com.br
>
> Atenciosamente,
> Equipe MCP-RH

---

## 5. Políticas e Termos

### 5.1 Política de Privacidade

#### 5.1.1 Estrutura da Política

**Seções obrigatórias (LGPD Art. 9º):**

1. **Identidade do Controlador e DPO**
   - Razão social do tenant (empresa cliente)
   - Contato do DPO do tenant
   - Contato do DPO do MCP-RH (operador)

2. **Dados Coletados**
   - Lista completa de dados pessoais
   - Identificação clara de dados sensíveis
   - Fonte dos dados (colaborador, RH, integração)

3. **Finalidades**
   - Gestão de PDI e desenvolvimento
   - Avaliação de desempenho BSC
   - Análise comportamental DISC (com consentimento)
   - Pesquisa de clima (com consentimento)
   - Recomendações de IA
   - Compliance e auditoria

4. **Base Legal**
   - Mapeamento conforme seção 1.2 deste documento

5. **Compartilhamento**
   - Integrações MCP habilitadas (com finalidade)
   - Prestadores de serviço (cloud, suporte)
   - Transferência internacional (se aplicável + cláusulas contratuais)

6. **Retenção**
   - Prazos por categoria de dados
   - Critérios de eliminação

7. **Direitos do Titular**
   - Como exercer (formulário, e-mail)
   - Prazos de resposta
   - Gratuidade

8. **Segurança**
   - Medidas técnicas (criptografia, controle de acesso)
   - Medidas organizacionais (treinamento, políticas)

9. **Cookies e Rastreamento**
   - Tipos de cookies (essenciais, analíticos)
   - Opção de opt-out

10. **Alterações da Política**
    - Notificação prévia de mudanças materiais
    - Histórico de versões

#### 5.1.2 Transparência e Linguagem

**Princípios:**
- ✅ Linguagem clara e acessível (não apenas juridiquês)
- ✅ Exemplos práticos
- ✅ Ícones visuais para facilitar compreensão
- ✅ Versão resumida + versão completa
- ✅ Disponível em local de fácil acesso

**Exemplo de seção clara:**

> **Por que coletamos seu perfil DISC?**
>
> O DISC ajuda a entender como você se comunica e toma decisões. Com isso, podemos:
> - Sugerir formas de comunicação mais eficazes com sua equipe
> - Recomendar treinamentos alinhados ao seu estilo
> - Ajudar seu líder a adaptar o acompanhamento
>
> ✅ Você decide se quer fazer o DISC. Sem isso, o sistema ainda funciona normalmente.
> ✅ Você pode revogar seu consentimento a qualquer momento em "Minhas Preferências".

### 5.2 Termos de Uso

#### 5.2.1 Escopo

**Aplicável a:**
- Colaboradores usuários do sistema
- Líderes e gestores
- BP/RH e coaches
- Administradores de tenant

**Principais cláusulas:**

1. **Aceitação dos Termos**
   - Obrigatoriedade de aceite para uso
   - Atualização mediante notificação

2. **Uso Adequado**
   - Proibição de uso para fins discriminatórios
   - Vedação de uso de dados para fins não autorizados
   - Responsabilidade por segurança de credenciais

3. **Propriedade Intelectual**
   - Software e conteúdo são propriedade do MCP-RH
   - Licença de uso (não transferência de propriedade)

4. **Limitação de Responsabilidade**
   - Disponibilidade (SLA de 99,5%)
   - Força maior e caso fortuito
   - Responsabilidade por uso inadequado de recomendações de IA

5. **Suspensão e Término**
   - Causas de suspensão de acesso
   - Processo de offboarding e exclusão de dados

6. **Foro e Legislação**
   - Foro de [cidade do tenant]
   - Legislação brasileira aplicável

### 5.3 SLA (Service Level Agreement)

#### 5.3.1 Níveis de Serviço

| Métrica | Commitment | Medição | Penalidade (se aplicável) |
|---------|------------|---------|---------------------------|
| **Disponibilidade** | 99,5% ao mês | Uptime mensal | Crédito proporcional |
| **Tempo de resposta (API)** | P95 < 500ms | APM contínuo | - |
| **Tempo de restauração (incidente SEV 1)** | < 4 horas | Ticket de incidente | SLA report |
| **Backup e recuperação** | RPO 1h, RTO 4h | Testes trimestrais | - |
| **Suporte (crítico)** | Resposta em 1h | Horário comercial | SLA report |
| **Suporte (não crítico)** | Resposta em 24h | Horário comercial | - |

**Exclusões de SLA:**
- Manutenção programada (notificada com 7 dias de antecedência)
- Força maior (desastres naturais, ataques DDoS massivos)
- Problemas no lado do cliente (rede, navegador)

#### 5.3.2 Janelas de Manutenção

**Programada:**
- Domingo, 02:00 - 06:00 (horário de Brasília)
- Máximo de 4 horas por mês
- Notificação prévia de 7 dias

**Emergencial:**
- Apenas para incidentes SEV 1
- Notificação imediata via status page

### 5.4 DPA (Data Processing Agreement)

#### 5.4.1 Finalidade do DPA

Acordo entre **Controlador (tenant)** e **Operador (MCP-RH)** que estabelece:
- Instruções de tratamento de dados
- Responsabilidades de cada parte
- Obrigações de segurança e conformidade
- Condições de subcontratação
- Transferência internacional (se aplicável)
- Auditoria e fiscalização

#### 5.4.2 Cláusulas Essenciais

**Cláusula 1: Objeto e Instrução**
- Operador tratará dados apenas conforme instruções documentadas do Controlador
- Escopo limitado a funcionalidades do sistema
- Proibição de uso para fins próprios

**Cláusula 2: Confidencialidade**
- Equipe do Operador assinará NDA
- Acesso apenas por colaboradores autorizados
- Treinamento em LGPD obrigatório

**Cláusula 3: Segurança**
- Controles técnicos conforme seção 3 deste documento
- Revisão anual de controles
- Certificações (ISO 27001, SOC 2 - meta)

**Cláusula 4: Suboperadores**
- Lista de suboperadores (cloud provider, suporte)
- Obrigação de DPA em cascata
- Notificação prévia de novos suboperadores

**Cláusula 5: Direitos dos Titulares**
- Operador auxiliará Controlador no atendimento de requisições
- Prazo de resposta: 5 dias úteis
- Ferramentas automatizadas para export/exclusão

**Cláusula 6: Incidentes**
- Notificação ao Controlador em até 24h
- Cooperação na investigação e notificação à ANPD
- Relatório pós-incidente

**Cláusula 7: Auditoria**
- Controlador pode auditar Operador (anualmente ou após incidente)
- Relatórios SOC 2 / ISO 27001 como evidência
- Acesso a logs de auditoria

**Cláusula 8: Término**
- Devolução ou eliminação de dados em até 30 dias
- Certificado de destruição assinado por DPO
- Exceção para logs legais (retenção obrigatória)

**Cláusula 9: Transferência Internacional**
- Cláusulas Contratuais Padrão da UE (se aplicável)
- Garantias de nível adequado de proteção
- Notificação e consentimento do Controlador

**Cláusula 10: Responsabilidade e Indenização**
- Operador responsável por danos causados por tratamento em violação à LGPD
- Limite de responsabilidade (conforme contrato comercial)
- Seguro cyber (desejável)

---

## 6. Riscos e Mitigações

### 6.1 Matriz de Riscos

| ID | Risco | Probabilidade | Impacto | Nível | Controles Mitigadores |
|----|-------|---------------|---------|-------|------------------------|
| **R01** | Vazamento de dados sensíveis (DISC, clima) por falha de segregação multi-tenant | Baixa | Crítico | **ALTO** | Isolamento lógico, testes automatizados, pentest trimestral |
| **R02** | Uso de dados comportamentais para decisões discriminatórias | Média | Alto | **ALTO** | Guardrails, auditoria de uso, IA como copiloto (não decisor) |
| **R03** | Acesso não autorizado a dados pessoais por comprometimento de credenciais | Média | Alto | **ALTO** | MFA obrigatório, monitoramento de acesso, revisão trimestral |
| **R04** | Perda de dados por falha de backup | Baixa | Crítico | **MÉDIO** | Backup automatizado, testes de restore trimestrais, redundância |
| **R05** | Incidente de segurança (ransomware, intrusão) | Baixa | Crítico | **ALTO** | EDR, SIEM, SOC 24x7, plano de resposta a incidentes |
| **R06** | Não conformidade com LGPD (falta de consentimento, retenção excessiva) | Média | Alto | **ALTO** | DPO ativo, ROPA atualizado, automação de retenção, treinamento |
| **R07** | Indisponibilidade prolongada afetando gestão de pessoas | Baixa | Médio | **MÉDIO** | Arquitetura resiliente, SLA 99,5%, DR plan |
| **R08** | Recomendações de IA inadequadas ou discriminatórias | Média | Alto | **ALTO** | Revisão humana obrigatória, explainability, bias testing |
| **R09** | Transferência internacional sem garantias adequadas | Baixa | Alto | **MÉDIO** | Cláusulas contratuais padrão, validação jurídica |
| **R10** | Cross-tenant data leakage por bug de código | Baixa | Crítico | **ALTO** | Global query filter, code review, testes de isolamento |
| **R11** | Uso de PDI como ferramenta punitiva | Média | Médio | **MÉDIO** | Políticas de uso, treinamento de líderes, auditoria de uso |
| **R12** | Falta de auditoria em ações sensíveis | Baixa | Alto | **MÉDIO** | Logs obrigatórios, SIEM, relatórios automáticos |

### 6.2 Controles Implementados

#### 6.2.1 Controles Técnicos

| Controle | Tipo | Status | Efetividade | Responsável |
|----------|------|--------|-------------|-------------|
| Criptografia TLS 1.3 em trânsito | Preventivo | ✅ Implementado | Alta | Infra |
| Criptografia AES-256 em repouso | Preventivo | ✅ Implementado | Alta | Infra + Dev |
| Autenticação Multi-Fator (MFA) | Preventivo | ✅ Obrigatório para admins | Alta | Identity |
| RBAC granular por tenant e módulo | Preventivo | ✅ Implementado | Alta | Dev |
| Global query filter (multi-tenant) | Preventivo | ✅ Implementado | Alta | Dev |
| Logs de auditoria centralizados | Detectivo | ✅ Implementado | Alta | DevOps |
| SIEM com alertas em tempo real | Detectivo | 🔄 Em implementação | Alta | SecOps |
| Backup automatizado (RPO 1h) | Recuperação | ✅ Implementado | Alta | Infra |
| Testes de penetração | Detectivo | 🔄 Trimestral | Média | SecOps |
| WAF (Web Application Firewall) | Preventivo | ✅ Implementado | Média | Infra |
| Rate limiting e anti-DDoS | Preventivo | ✅ Implementado | Média | Infra |
| Sanitização de inputs (anti-injection) | Preventivo | ✅ Implementado | Alta | Dev |
| Dependency scanning (vulnerabilidades) | Detectivo | ✅ CI/CD pipeline | Alta | DevOps |

#### 6.2.2 Controles Organizacionais

| Controle | Tipo | Frequência | Responsável |
|----------|------|------------|-------------|
| Treinamento LGPD para equipe | Preventivo | Anual + onboarding | DPO |
| Revisão de acessos privilegiados | Detectivo | Mensal | CISO |
| Avaliação de risco (risk assessment) | Preventivo | Anual | DPO + CISO |
| DPIA para novas funcionalidades | Preventivo | Por demanda | DPO |
| Política de segurança da informação | Preventivo | Revisão anual | CISO |
| Plano de resposta a incidentes | Recuperação | Revisão semestral | CISO |
| Testes de restore de backup | Recuperação | Trimestral | Infra |
| Simulação de incidente (tabletop) | Detectivo | Semestral | CISO + DPO |

### 6.3 Plano de Continuidade de Negócios (BCP)

#### 6.3.1 Cenários de Desastre

| Cenário | Probabilidade | Impacto | Estratégia de Recuperação |
|---------|---------------|---------|---------------------------|
| **Falha de datacenter (região única)** | Baixa | Crítico | Failover automático para região secundária (multi-region) |
| **Ransomware / corrupção de dados** | Baixa | Crítico | Restore de backup (RPO 1h, RTO 4h) + isolamento de rede |
| **Perda de acesso a cloud provider** | Muito Baixa | Crítico | Multi-cloud strategy (ativo-passivo) |
| **Comprometimento de credenciais de admin** | Média | Alto | Revogação imediata, rotação de secrets, análise forense |
| **Desastre natural (incêndio, inundação)** | Muito Baixa | Crítico | Cloud-first (sem dependência de on-premises) |
| **Ataque DDoS massivo** | Média | Médio | CDN, rate limiting, escalabilidade automática |

#### 6.3.2 Procedimentos de Recuperação

**RTO (Recovery Time Objective):** 4 horas
**RPO (Recovery Point Objective):** 1 hora

**Passos de DR (Disaster Recovery):**

1. **Detecção (T+0)**
   - Alerta automático de SIEM ou monitoramento
   - Acionamento do time de resposta

2. **Avaliação (T+15min)**
   - Classificar tipo e severidade
   - Decidir: restore parcial ou failover completo

3. **Ativação de DR (T+30min)**
   - Failover para região secundária (se aplicável)
   - Restore de banco de dados a partir do último backup

4. **Validação (T+2h)**
   - Testes de smoke (funcionalidades críticas)
   - Validação de integridade de dados
   - Comunicação a stakeholders

5. **Normalização (T+4h)**
   - Sistema em operação normal
   - Post-mortem agendado

#### 6.3.3 Comunicação de Crise

**Stakeholders a notificar:**
1. **Imediato (15 min):** CISO, DPO, CTO
2. **1 hora:** CEO, Tenant Admins afetados
3. **4 horas:** Todos os usuários afetados (via e-mail + status page)
4. **24 horas:** ANPD (se houver vazamento de dados)

**Canais:**
- Status page pública: status.mcprh.com.br
- E-mail para admins de tenant
- SMS para contatos de emergência
- WhatsApp para grupo de crise

**Template de comunicação:**

> **[STATUS] Incidente de disponibilidade - MCP-RH**
>
> **Data/Hora:** 13/04/2026 14:32 UTC-3
> **Severidade:** SEV 1 - Crítico
> **Status:** Em investigação
>
> **Resumo:**
> Identificamos indisponibilidade parcial do sistema MCP-RH desde 14:15. Nossas equipes estão trabalhando na recuperação.
>
> **Impacto:**
> - Módulos afetados: PDI, DISC
> - Tenants afetados: todos
> - Dados: não há evidência de vazamento
>
> **Próxima atualização:** 15:30 UTC-3
>
> Acompanhe em: status.mcprh.com.br

---

## 7. Compliance Checklist

### 7.1 Checklist LGPD

| Requisito | Status | Evidência | Responsável |
|-----------|--------|-----------|-------------|
| Mapeamento de dados pessoais | ✅ | ROPA (Registro de Operações) | DPO |
| Base legal definida para cada tratamento | ✅ | Matriz de base legal (seção 1.2) | DPO |
| Consentimento para dados sensíveis | ✅ | Fluxo de opt-in implementado | Dev + DPO |
| Possibilidade de revogação de consentimento | ✅ | Botão "Revogar" no perfil | Dev |
| Políticas de retenção e eliminação | ✅ | Política documentada + automação | DPO + Infra |
| Direitos dos titulares implementados | ✅ | Formulário de requisições | Dev + DPO |
| DPO nomeado e publicado | ✅ | dpo@mcprh.com.br | Jurídico |
| Registro de operações de tratamento (ROPA) | ✅ | Documento interno atualizado | DPO |
| DPA com suboperadores | 🔄 | Em revisão jurídica | Jurídico |
| Política de privacidade publicada | ✅ | mcprh.com.br/privacidade | DPO + Marketing |
| Logs de auditoria para dados sensíveis | ✅ | SIEM configurado | DevOps |
| Criptografia de dados sensíveis | ✅ | AES-256 em repouso, TLS 1.3 em trânsito | Infra |
| Plano de resposta a incidentes | ✅ | Runbook documentado | CISO |
| Treinamento LGPD para equipe | ✅ | Plataforma de e-learning | RH + DPO |
| DPIA para tratamentos de alto risco | 🔄 | Template criado, em aplicação | DPO |
| Transferência internacional com garantias | 🔄 | Cláusulas contratuais (se aplicável) | Jurídico |

**Legenda:**
✅ Implementado | 🔄 Em progresso | ❌ Não iniciado

### 7.2 Checklist de Segurança (ISO 27001 / NIST)

| Domínio | Controle | Status | Observação |
|---------|----------|--------|------------|
| **Acesso** | MFA para contas privilegiadas | ✅ | Obrigatório para admin e DPO |
| **Acesso** | RBAC granular | ✅ | Por tenant e módulo |
| **Acesso** | Revisão trimestral de acessos | ✅ | Automatizada |
| **Criptografia** | TLS 1.3 | ✅ | HSTS habilitado |
| **Criptografia** | AES-256 em repouso | ✅ | Banco e storage |
| **Criptografia** | Key management em Vault | ✅ | Azure Key Vault |
| **Auditoria** | Logs centralizados | ✅ | SIEM |
| **Auditoria** | Retenção de 6 meses (mínimo) | ✅ | 2 anos para dados sensíveis |
| **Rede** | Firewall e WAF | ✅ | Azure Firewall + WAF |
| **Rede** | Segmentação por ambiente | ✅ | Prod, staging, dev isolados |
| **Aplicação** | Input validation | ✅ | Anti-injection |
| **Aplicação** | Dependency scanning | ✅ | CI/CD pipeline |
| **Backup** | Backup automatizado | ✅ | RPO 1h, RTO 4h |
| **Backup** | Testes de restore | ✅ | Trimestral |
| **Incidentes** | Plano de resposta | ✅ | Revisado semestralmente |
| **Incidentes** | Simulação (tabletop) | 🔄 | Próxima: Q3/2026 |
| **Pessoas** | Treinamento de segurança | ✅ | Anual + onboarding |
| **Pessoas** | Background check | 🔄 | Para acessos críticos |

---

## 8. KPIs de Governança e Compliance

### 8.1 Indicadores de Compliance

| KPI | Meta | Medição | Frequência |
|-----|------|---------|------------|
| **Taxa de atendimento de requisições LGPD no prazo** | > 95% | (Atendidas no prazo / Total) × 100 | Mensal |
| **Tempo médio de resposta a titular** | < 10 dias | Média de dias entre solicitação e resposta | Mensal |
| **Incidentes de vazamento de dados** | 0 | Contagem de SEV 1 com vazamento | Mensal |
| **Taxa de consentimento para DISC** | Monitorar | (Opt-ins / Usuários elegíveis) × 100 | Mensal |
| **Revogações de consentimento** | Monitorar | Contagem mensal | Mensal |
| **Completude do ROPA** | 100% | % de tratamentos mapeados | Trimestral |
| **Aderência a política de retenção** | 100% | % de dados eliminados no prazo | Mensal |

### 8.2 Indicadores de Segurança

| KPI | Meta | Medição | Frequência |
|-----|------|---------|------------|
| **Uptime (SLA)** | > 99,5% | Monitoramento APM | Mensal |
| **Tempo de resposta a incidente SEV 1** | < 15 min | Média de tempo de triagem | Mensal |
| **Taxa de adoção de MFA** | 100% (admins) | % de usuários com MFA ativo | Mensal |
| **Vulnerabilidades críticas não remediadas** | 0 | Contagem de CVEs críticos abertos > 7 dias | Semanal |
| **Tentativas de acesso não autorizado** | Monitorar | Contagem de eventos de alerta | Semanal |
| **Sucesso de testes de backup** | 100% | % de restores bem-sucedidos | Trimestral |
| **Cobertura de testes de segurança** | > 80% | % de código com security tests | Mensal |

### 8.3 Indicadores de Auditoria

| KPI | Meta | Medição | Frequência |
|-----|------|---------|------------|
| **Taxa de revisão de acessos concluída** | 100% | % de revisões concluídas no prazo | Mensal |
| **Acessos privilegiados não revisados** | 0 | Contagem de acessos > 30 dias sem revisão | Mensal |
| **Logs com anomalias investigadas** | 100% | % de alertas SIEM investigados | Semanal |
| **Completude de audit trail** | > 99% | % de eventos críticos logados | Diário |

---

## 9. Roadmap de Compliance

### 9.1 Curto Prazo (0-6 meses)

- ✅ **Q2/2026**
  - Publicar Política de Privacidade versão 1.0
  - Implementar fluxo de consentimento para DISC e clima
  - Configurar logs de auditoria centralizados (SIEM)
  - Treinar equipe em LGPD básico

- 🔄 **Q3/2026**
  - Obter certificação ISO 27001 (planejamento)
  - Realizar primeiro pentest externo
  - Implementar automação de retenção de dados
  - Revisar e assinar DPAs com todos os tenants ativos

### 9.2 Médio Prazo (6-12 meses)

- 📅 **Q4/2026**
  - Certificação SOC 2 Type I
  - Implementar DPIA automatizado para novas features
  - Expandir treinamento LGPD para tenants (BP/RH)
  - Implementar Privacy Dashboard (visibilidade de tratamento para colaboradores)

- 📅 **Q1/2027**
  - Certificação ISO 27001
  - Implementar Privacy by Design em 100% de novas features
  - Automatizar detecção de dados sensíveis não mapeados (DLP)

### 9.3 Longo Prazo (12-24 meses)

- 📅 **2027-2028**
  - Certificação SOC 2 Type II
  - Compliance com regulações internacionais (GDPR - se expandir para EU)
  - Implementar IA para detecção de anomalias em auditoria
  - Obter selo de conformidade LGPD (se disponível pela ANPD)

---

## 10. Contatos e Responsabilidades

### 10.1 Estrutura de Governança

```
┌─────────────────────────────────────┐
│          CEO / C-Level              │
└─────────────────┬───────────────────┘
                  │
    ┌─────────────┼─────────────┐
    │             │             │
┌───▼────┐   ┌───▼────┐   ┌───▼────┐
│  CISO  │   │  DPO   │   │  CTO   │
└───┬────┘   └───┬────┘   └───┬────┘
    │            │            │
    │        ┌───▼────┐       │
    │        │ Jurídico│      │
    │        └────────┘       │
    │                         │
┌───▼────────────────────┐    │
│  Equipe de Segurança   │    │
│  - SOC                 │    │
│  - SecOps              │    │
│  - Compliance          │    │
└────────────────────────┘    │
                              │
                         ┌────▼─────────┐
                         │ Arquitetura  │
                         │ DevOps       │
                         │ Desenvolvimento│
                         └──────────────┘
```

### 10.2 Contatos de Emergência

| Papel | Nome | E-mail | Telefone | Disponibilidade |
|-------|------|--------|----------|-----------------|
| **CISO** | [Nome] | ciso@mcprh.com.br | +55 11 9xxxx-xxxx | 24/7 (emergência) |
| **DPO** | [Nome] | dpo@mcprh.com.br | +55 11 9xxxx-xxxx | Horário comercial + emergência |
| **CTO** | [Nome] | cto@mcprh.com.br | +55 11 9xxxx-xxxx | 24/7 (emergência) |
| **Jurídico** | [Nome] | juridico@mcprh.com.br | +55 11 9xxxx-xxxx | Horário comercial |
| **SOC (plantão)** | - | soc@mcprh.com.br | +55 11 9xxxx-xxxx | 24/7 |

**Canais de comunicação de crise:**
- WhatsApp: Grupo "MCP-RH Crise"
- Telefone: Linha direta de emergência
- E-mail: crise@mcprh.com.br

---

## 11. Revisão e Atualização

### 11.1 Controle de Versões

| Versão | Data | Autor | Mudanças |
|--------|------|-------|----------|
| 1.0 | 2026-04-13 | DPO + CISO | Versão inicial |

### 11.2 Frequência de Revisão

- **Revisão ordinária:** Anual (Q1 de cada ano)
- **Revisão extraordinária:**
  - Mudanças na LGPD ou regulamentação
  - Incidente de segurança SEV 1
  - Mudança significativa de arquitetura
  - Solicitação de auditoria ou ANPD

### 11.3 Aprovações

**Este documento foi revisado e aprovado por:**

- [ ] DPO (Data Protection Officer)
- [ ] CISO (Chief Information Security Officer)
- [ ] Jurídico
- [ ] CTO
- [ ] CEO

**Data de aprovação:** _____________

---

## 12. Anexos

### 12.1 Glossário

| Termo | Definição |
|-------|-----------|
| **LGPD** | Lei Geral de Proteção de Dados (Lei 13.709/2018) |
| **ANPD** | Autoridade Nacional de Proteção de Dados |
| **DPO** | Data Protection Officer (Encarregado de Dados) |
| **Controlador** | Quem decide finalidade e meios de tratamento (tenant) |
| **Operador** | Quem trata dados conforme instruções do controlador (MCP-RH) |
| **ROPA** | Record of Processing Activities (Registro de Operações) |
| **DPIA** | Data Protection Impact Assessment (Avaliação de Impacto) |
| **RBAC** | Role-Based Access Control |
| **MFA** | Multi-Factor Authentication |
| **SIEM** | Security Information and Event Management |
| **TLS** | Transport Layer Security |
| **AES** | Advanced Encryption Standard |
| **RPO** | Recovery Point Objective |
| **RTO** | Recovery Time Objective |
| **WAF** | Web Application Firewall |
| **SOC** | Security Operations Center |

### 12.2 Referências Normativas

- **Lei 13.709/2018** - Lei Geral de Proteção de Dados (LGPD)
- **Resolução ANPD nº 2/2022** - Agentes de Tratamento de Pequeno Porte
- **ISO/IEC 27001:2022** - Segurança da Informação
- **ISO/IEC 27701:2019** - Privacy Information Management
- **NIST Cybersecurity Framework** - Framework de Cibersegurança
- **SOC 2 Type II** - Controles de Segurança, Disponibilidade e Confidencialidade
- **OWASP Top 10** - Principais Riscos de Segurança em Aplicações Web

### 12.3 Templates e Formulários

**Disponíveis internamente:**
- Template de DPIA (Data Protection Impact Assessment)
- Formulário de requisição de direitos LGPD
- Checklist de revisão de código (segurança)
- Template de DPA (Data Processing Agreement)
- Formulário de reporte de incidente
- Template de comunicação de violação à ANPD
- Checklist de offboarding (eliminação de dados)

---

**Documento controlado - Confidencial**
**MCP-RH - Sistema de Gestão de Pessoas**
**Versão 1.0 - Abril de 2026**

---

## Resumo para Apresentação Executiva

### 🎯 Objetivo
Garantir compliance LGPD, segurança da informação e governança robusta para proteger dados sensíveis de colaboradores (DISC, clima, PDI, desempenho).

### 🛡️ Principais Controles
1. **Compliance LGPD**
   - ✅ Base legal mapeada para todos os tratamentos
   - ✅ Consentimento explícito para dados sensíveis (DISC, clima)
   - ✅ Direitos dos titulares implementados (acesso, exclusão, portabilidade)
   - ✅ DPO nomeado e acessível

2. **Segurança**
   - ✅ Criptografia AES-256 (repouso) + TLS 1.3 (trânsito)
   - ✅ RBAC granular + MFA obrigatório para admins
   - ✅ Isolamento multi-tenant rigoroso
   - ✅ Logs de auditoria para dados sensíveis (2 anos)

3. **Governança**
   - ✅ Processo de aprovação para acessos críticos
   - ✅ Revisão mensal de acessos privilegiados
   - ✅ Plano de resposta a incidentes (< 15 min)
   - ✅ SLA 99,5% com DR (RPO 1h, RTO 4h)

### 📊 KPIs de Sucesso
- Taxa de atendimento LGPD no prazo: > 95%
- Incidentes de vazamento: 0
- Uptime: > 99,5%
- Vulnerabilidades críticas abertas: 0

### 🚀 Próximos Passos
- Q2/2026: Publicar Política de Privacidade
- Q3/2026: Primeiro pentest externo
- Q4/2026: Iniciar certificação ISO 27001
- Q1/2027: Obter SOC 2 Type I

**Status geral: 🟢 Em conformidade com LGPD e boas práticas de segurança**
