# Modelo de Dados - MCP RH

## Visão Geral

Este documento descreve o modelo de dados completo do MCP RH, uma plataforma multi-tenant para gestão de desenvolvimento humano, PDI, DISC, BSC e Escuta Coletiva.

### Princípios do Modelo

- **Multi-tenant por design**: Isolamento total de dados entre tenants
- **Auditoria completa**: Rastreamento de todas operações sensíveis
- **Domínios separados**: Separação clara entre dados humanos e operacionais
- **Permissões granulares**: Controle por papel e contexto
- **Soft delete**: Preservação de histórico com exclusão lógica

---

## 1. Diagrama Conceitual

### 1.1 Entidades Principais e Relacionamentos

```
TENANT (1) ────┬──── (N) PERSON
               │
               ├──── (N) ROLE
               │
               ├──── (N) ORGANIZATIONAL_UNIT
               │
               ├──── (N) COMPETENCY
               │
               ├──── (N) BSC_OBJECTIVE
               │
               └──── (N) AUDIT_LOG

PERSON (1) ────┬──── (1) DISC_PROFILE
               │
               ├──── (N) PDI_CYCLE
               │
               ├──── (N) FEEDBACK
               │
               ├──── (N) ONE_ON_ONE
               │
               ├──── (N) PULSE_RESPONSE
               │
               ├──── (N) ORGANIZATIONAL_ASSESSMENT
               │
               └──── (N) PERSON_COMPETENCY

PDI_CYCLE (1) ─┬──── (N) PDI_GOAL
               │
               ├──── (N) PDI_ACTION
               │
               └──── (N) PDI_CHECKPOINT

COMPETENCY (1) ─── (N) PERSON_COMPETENCY
               └─── (N) PDI_GOAL

BSC_OBJECTIVE (1) ─┬─── (N) BSC_INDICATOR
                   └─── (N) PERSON_GOAL

ORGANIZATIONAL_UNIT (1) ─── (N) PERSON (líder)
                         └─── (N) PERSON (membros)
```

### 1.2 Relacionamentos N:N

```
PERSON (N) ────── (N) ROLE
  └─── via: PERSON_ROLE

COMPETENCY (N) ── (N) ROLE
  └─── via: ROLE_COMPETENCY

PDI_GOAL (N) ──── (N) COMPETENCY
  └─── via: goal_competencies (array ou tabela associativa)
```

---

## 2. Modelo Lógico - Schemas Completos

### 2.1 Domínio: Tenant e Organização

#### Tabela: `tenants`

Representa cada cliente/organização do sistema.

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único do tenant |
| `slug` | VARCHAR(100) | UNIQUE, NOT NULL | Slug URL-friendly (ex: "empresa-abc") |
| `name` | VARCHAR(200) | NOT NULL | Nome da organização |
| `status` | ENUM | NOT NULL | active, suspended, inactive |
| `subscription_tier` | ENUM | NOT NULL | free, basic, professional, enterprise |
| `max_users` | INTEGER | NOT NULL | Limite de usuários permitidos |
| `settings` | JSONB | NULL | Configurações específicas do tenant |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |
| `updated_at` | TIMESTAMP | NOT NULL | Data de atualização |
| `deleted_at` | TIMESTAMP | NULL | Soft delete |

**Índices:**
- `idx_tenants_slug` (slug)
- `idx_tenants_status` (status)

**Constraints:**
- `chk_max_users_positive`: max_users > 0

---

#### Tabela: `organizational_units`

Áreas, departamentos ou times dentro de um tenant.

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `tenant_id` | UUID | FK, NOT NULL | Referência ao tenant |
| `parent_id` | UUID | FK, NULL | Unidade pai (hierarquia) |
| `name` | VARCHAR(200) | NOT NULL | Nome da unidade |
| `code` | VARCHAR(50) | NULL | Código interno (ex: "DEV-001") |
| `type` | ENUM | NOT NULL | department, team, area, division |
| `leader_id` | UUID | FK, NULL | Pessoa líder da unidade |
| `description` | TEXT | NULL | Descrição da unidade |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |
| `updated_at` | TIMESTAMP | NOT NULL | Data de atualização |
| `deleted_at` | TIMESTAMP | NULL | Soft delete |

**Índices:**
- `idx_org_units_tenant` (tenant_id)
- `idx_org_units_parent` (parent_id)
- `idx_org_units_leader` (leader_id)

**Constraints:**
- FK `tenant_id` → `tenants(id)` ON DELETE CASCADE
- FK `parent_id` → `organizational_units(id)` ON DELETE SET NULL
- FK `leader_id` → `persons(id)` ON DELETE SET NULL

---

### 2.2 Domínio: Identidade e Acesso

#### Tabela: `persons`

Representa todas as pessoas no sistema (colaboradores, coaches, BPs, líderes).

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `tenant_id` | UUID | FK, NOT NULL | Referência ao tenant |
| `email` | VARCHAR(255) | NOT NULL | Email único dentro do tenant |
| `full_name` | VARCHAR(200) | NOT NULL | Nome completo |
| `preferred_name` | VARCHAR(100) | NULL | Nome preferido |
| `document_number` | VARCHAR(50) | NULL | CPF, passaporte, etc |
| `phone` | VARCHAR(20) | NULL | Telefone |
| `hire_date` | DATE | NULL | Data de admissão |
| `birth_date` | DATE | NULL | Data de nascimento |
| `organizational_unit_id` | UUID | FK, NULL | Unidade organizacional |
| `manager_id` | UUID | FK, NULL | Gestor direto |
| `position_title` | VARCHAR(200) | NULL | Cargo/posição |
| `employment_status` | ENUM | NOT NULL | active, on_leave, terminated |
| `profile_photo_url` | VARCHAR(500) | NULL | URL da foto de perfil |
| `timezone` | VARCHAR(50) | NULL | Fuso horário |
| `language` | VARCHAR(10) | NULL | pt-BR, en-US, etc |
| `settings` | JSONB | NULL | Preferências do usuário |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |
| `updated_at` | TIMESTAMP | NOT NULL | Data de atualização |
| `deleted_at` | TIMESTAMP | NULL | Soft delete |

**Índices:**
- `idx_persons_tenant_email` (tenant_id, email) UNIQUE WHERE deleted_at IS NULL
- `idx_persons_org_unit` (organizational_unit_id)
- `idx_persons_manager` (manager_id)
- `idx_persons_status` (employment_status)

**Constraints:**
- FK `tenant_id` → `tenants(id)` ON DELETE CASCADE
- FK `organizational_unit_id` → `organizational_units(id)` ON DELETE SET NULL
- FK `manager_id` → `persons(id)` ON DELETE SET NULL

---

#### Tabela: `roles`

Papéis e permissões no sistema.

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `tenant_id` | UUID | FK, NOT NULL | Referência ao tenant |
| `name` | VARCHAR(100) | NOT NULL | Nome do papel |
| `code` | VARCHAR(50) | NOT NULL | Código único (ex: "BP", "COACH") |
| `description` | TEXT | NULL | Descrição do papel |
| `permissions` | JSONB | NOT NULL | Array de permissões |
| `is_system_role` | BOOLEAN | NOT NULL, DEFAULT false | Se é papel do sistema |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |
| `updated_at` | TIMESTAMP | NOT NULL | Data de atualização |

**Índices:**
- `idx_roles_tenant_code` (tenant_id, code) UNIQUE

**Constraints:**
- FK `tenant_id` → `tenants(id)` ON DELETE CASCADE

---

#### Tabela: `person_roles`

Associação entre pessoas e papéis (N:N).

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `person_id` | UUID | FK, NOT NULL | Referência à pessoa |
| `role_id` | UUID | FK, NOT NULL | Referência ao papel |
| `scope` | VARCHAR(50) | NULL | tenant, unit, team |
| `scope_id` | UUID | NULL | ID da unidade/time se aplicável |
| `granted_at` | TIMESTAMP | NOT NULL | Quando foi concedido |
| `granted_by` | UUID | FK, NULL | Quem concedeu |
| `revoked_at` | TIMESTAMP | NULL | Quando foi revogado |

**Índices:**
- `idx_person_roles_person` (person_id)
- `idx_person_roles_role` (role_id)
- `idx_person_roles_active` (person_id, role_id) WHERE revoked_at IS NULL

**Constraints:**
- FK `person_id` → `persons(id)` ON DELETE CASCADE
- FK `role_id` → `roles(id)` ON DELETE CASCADE
- FK `granted_by` → `persons(id)` ON DELETE SET NULL

---

### 2.3 Domínio: Perfil Comportamental (DISC)

#### Tabela: `disc_profiles`

Perfil DISC de cada pessoa.

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `person_id` | UUID | FK, NOT NULL, UNIQUE | Referência à pessoa |
| `assessment_date` | DATE | NOT NULL | Data da avaliação |
| `d_score` | DECIMAL(5,2) | NOT NULL | Score Dominância (0-100) |
| `i_score` | DECIMAL(5,2) | NOT NULL | Score Influência (0-100) |
| `s_score` | DECIMAL(5,2) | NOT NULL | Score Estabilidade (0-100) |
| `c_score` | DECIMAL(5,2) | NOT NULL | Score Conformidade (0-100) |
| `primary_style` | ENUM | NOT NULL | D, I, S, C |
| `secondary_style` | ENUM | NULL | D, I, S, C |
| `profile_combination` | VARCHAR(10) | NOT NULL | Ex: "DI", "SC", "CD" |
| `interpretation` | TEXT | NULL | Interpretação do perfil |
| `strengths` | JSONB | NULL | Array de forças |
| `development_areas` | JSONB | NULL | Array de áreas de desenvolvimento |
| `communication_tips` | JSONB | NULL | Dicas de comunicação |
| `stress_indicators` | JSONB | NULL | Indicadores de estresse |
| `assessed_by` | UUID | FK, NULL | Quem aplicou/analisou |
| `raw_data` | JSONB | NULL | Dados brutos da avaliação |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |
| `updated_at` | TIMESTAMP | NOT NULL | Data de atualização |

**Índices:**
- `idx_disc_person` (person_id) UNIQUE
- `idx_disc_primary` (primary_style)

**Constraints:**
- FK `person_id` → `persons(id)` ON DELETE CASCADE
- FK `assessed_by` → `persons(id)` ON DELETE SET NULL
- `chk_disc_scores`: d_score BETWEEN 0 AND 100, etc.

---

### 2.4 Domínio: Escuta Coletiva e Consciência Organizacional

#### Tabela: `organizational_assessments`

Avaliações de consciência organizacional (EC - Escuta Coletiva).

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `tenant_id` | UUID | FK, NOT NULL | Referência ao tenant |
| `organizational_unit_id` | UUID | FK, NULL | Unidade específica ou NULL (toda org) |
| `assessment_type` | ENUM | NOT NULL | collective_listening, climate_survey, maturity |
| `title` | VARCHAR(200) | NOT NULL | Título da avaliação |
| `description` | TEXT | NULL | Descrição |
| `start_date` | DATE | NOT NULL | Data de início |
| `end_date` | DATE | NOT NULL | Data de término |
| `status` | ENUM | NOT NULL | draft, active, closed, archived |
| `consciousness_level` | INTEGER | NULL | Nível 1-7 (Modelo Graves) |
| `participation_rate` | DECIMAL(5,2) | NULL | Taxa de participação (%) |
| `overall_score` | DECIMAL(5,2) | NULL | Score geral |
| `dimensions` | JSONB | NULL | Dimensões avaliadas e scores |
| `findings` | TEXT | NULL | Principais achados |
| `recommendations` | TEXT | NULL | Recomendações |
| `conducted_by` | UUID | FK, NULL | Coach/consultor responsável |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |
| `updated_at` | TIMESTAMP | NOT NULL | Data de atualização |
| `deleted_at` | TIMESTAMP | NULL | Soft delete |

**Índices:**
- `idx_org_assess_tenant` (tenant_id)
- `idx_org_assess_unit` (organizational_unit_id)
- `idx_org_assess_status` (status)
- `idx_org_assess_dates` (start_date, end_date)

**Constraints:**
- FK `tenant_id` → `tenants(id)` ON DELETE CASCADE
- FK `organizational_unit_id` → `organizational_units(id)` ON DELETE SET NULL
- FK `conducted_by` → `persons(id)` ON DELETE SET NULL
- `chk_dates`: end_date >= start_date

---

#### Tabela: `pulse_surveys`

Pesquisas de pulso (clima, energia, humor).

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `tenant_id` | UUID | FK, NOT NULL | Referência ao tenant |
| `organizational_unit_id` | UUID | FK, NULL | Unidade específica ou NULL |
| `title` | VARCHAR(200) | NOT NULL | Título da pesquisa |
| `type` | ENUM | NOT NULL | mood, energy, engagement, risk |
| `frequency` | ENUM | NOT NULL | daily, weekly, biweekly, monthly |
| `questions` | JSONB | NOT NULL | Array de perguntas |
| `is_anonymous` | BOOLEAN | NOT NULL, DEFAULT true | Se é anônima |
| `status` | ENUM | NOT NULL | active, paused, completed |
| `start_date` | DATE | NOT NULL | Data de início |
| `end_date` | DATE | NULL | Data de término (NULL = contínua) |
| `created_by` | UUID | FK, NOT NULL | Criador da pesquisa |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |
| `updated_at` | TIMESTAMP | NOT NULL | Data de atualização |
| `deleted_at` | TIMESTAMP | NULL | Soft delete |

**Índices:**
- `idx_pulse_tenant` (tenant_id)
- `idx_pulse_unit` (organizational_unit_id)
- `idx_pulse_status` (status)

**Constraints:**
- FK `tenant_id` → `tenants(id)` ON DELETE CASCADE
- FK `organizational_unit_id` → `organizational_units(id)` ON DELETE SET NULL
- FK `created_by` → `persons(id)` ON DELETE CASCADE

---

#### Tabela: `pulse_responses`

Respostas individuais às pesquisas de pulso.

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `pulse_survey_id` | UUID | FK, NOT NULL | Referência à pesquisa |
| `person_id` | UUID | FK, NULL | NULL se anônima |
| `response_date` | TIMESTAMP | NOT NULL | Data/hora da resposta |
| `answers` | JSONB | NOT NULL | Respostas estruturadas |
| `overall_sentiment` | ENUM | NULL | very_negative, negative, neutral, positive, very_positive |
| `energy_level` | INTEGER | NULL | 1-10 |
| `metadata` | JSONB | NULL | Metadados (sem identificação) |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |

**Índices:**
- `idx_pulse_resp_survey` (pulse_survey_id)
- `idx_pulse_resp_person` (person_id)
- `idx_pulse_resp_date` (response_date)

**Constraints:**
- FK `pulse_survey_id` → `pulse_surveys(id)` ON DELETE CASCADE
- FK `person_id` → `persons(id)` ON DELETE SET NULL

---

### 2.5 Domínio: Competências

#### Tabela: `competencies`

Biblioteca de competências do tenant.

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `tenant_id` | UUID | FK, NOT NULL | Referência ao tenant |
| `code` | VARCHAR(50) | NOT NULL | Código único no tenant |
| `name` | VARCHAR(200) | NOT NULL | Nome da competência |
| `description` | TEXT | NULL | Descrição detalhada |
| `category` | ENUM | NOT NULL | technical, behavioral, leadership, core |
| `level_type` | ENUM | NOT NULL | numeric, descriptive, boolean |
| `level_definitions` | JSONB | NULL | Definições de níveis |
| `is_core` | BOOLEAN | NOT NULL, DEFAULT false | Se é competência core |
| `parent_competency_id` | UUID | FK, NULL | Competência pai (hierarquia) |
| `sort_order` | INTEGER | NULL | Ordem de exibição |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |
| `updated_at` | TIMESTAMP | NOT NULL | Data de atualização |
| `deleted_at` | TIMESTAMP | NULL | Soft delete |

**Índices:**
- `idx_comp_tenant_code` (tenant_id, code) UNIQUE WHERE deleted_at IS NULL
- `idx_comp_category` (category)
- `idx_comp_parent` (parent_competency_id)

**Constraints:**
- FK `tenant_id` → `tenants(id)` ON DELETE CASCADE
- FK `parent_competency_id` → `competencies(id)` ON DELETE SET NULL

---

#### Tabela: `role_competencies`

Competências esperadas por papel/cargo.

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `role_id` | UUID | FK, NOT NULL | Referência ao papel |
| `competency_id` | UUID | FK, NOT NULL | Referência à competência |
| `required_level` | INTEGER | NOT NULL | Nível esperado (1-5) |
| `is_mandatory` | BOOLEAN | NOT NULL, DEFAULT false | Se é mandatória |
| `weight` | DECIMAL(5,2) | NULL | Peso da competência (%) |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |

**Índices:**
- `idx_role_comp_role` (role_id)
- `idx_role_comp_unique` (role_id, competency_id) UNIQUE

**Constraints:**
- FK `role_id` → `roles(id)` ON DELETE CASCADE
- FK `competency_id` → `competencies(id)` ON DELETE CASCADE
- `chk_required_level`: required_level BETWEEN 1 AND 5

---

#### Tabela: `person_competencies`

Competências atuais de cada pessoa.

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `person_id` | UUID | FK, NOT NULL | Referência à pessoa |
| `competency_id` | UUID | FK, NOT NULL | Referência à competência |
| `current_level` | INTEGER | NOT NULL | Nível atual (1-5) |
| `target_level` | INTEGER | NULL | Nível desejado (1-5) |
| `last_assessed_date` | DATE | NULL | Data da última avaliação |
| `assessed_by` | UUID | FK, NULL | Quem avaliou |
| `evidence` | TEXT | NULL | Evidências do nível |
| `notes` | TEXT | NULL | Observações |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |
| `updated_at` | TIMESTAMP | NOT NULL | Data de atualização |

**Índices:**
- `idx_person_comp_person` (person_id)
- `idx_person_comp_unique` (person_id, competency_id) UNIQUE
- `idx_person_comp_gap` (person_id) WHERE target_level > current_level

**Constraints:**
- FK `person_id` → `persons(id)` ON DELETE CASCADE
- FK `competency_id` → `competencies(id)` ON DELETE CASCADE
- FK `assessed_by` → `persons(id)` ON DELETE SET NULL
- `chk_levels`: current_level BETWEEN 1 AND 5, target_level BETWEEN 1 AND 5

---

### 2.6 Domínio: BSC e Indicadores

#### Tabela: `bsc_perspectives`

Perspectivas do Balanced Scorecard.

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `tenant_id` | UUID | FK, NOT NULL | Referência ao tenant |
| `name` | VARCHAR(100) | NOT NULL | Nome da perspectiva |
| `code` | VARCHAR(50) | NOT NULL | Código (ex: FINANCIAL, CUSTOMER) |
| `description` | TEXT | NULL | Descrição |
| `sort_order` | INTEGER | NOT NULL | Ordem de exibição |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |
| `updated_at` | TIMESTAMP | NOT NULL | Data de atualização |

**Índices:**
- `idx_bsc_persp_tenant_code` (tenant_id, code) UNIQUE

**Constraints:**
- FK `tenant_id` → `tenants(id)` ON DELETE CASCADE

---

#### Tabela: `bsc_objectives`

Objetivos estratégicos do BSC.

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `tenant_id` | UUID | FK, NOT NULL | Referência ao tenant |
| `perspective_id` | UUID | FK, NOT NULL | Perspectiva BSC |
| `organizational_unit_id` | UUID | FK, NULL | Unidade responsável |
| `code` | VARCHAR(50) | NOT NULL | Código do objetivo |
| `title` | VARCHAR(200) | NOT NULL | Título do objetivo |
| `description` | TEXT | NULL | Descrição detalhada |
| `owner_id` | UUID | FK, NULL | Responsável pelo objetivo |
| `start_date` | DATE | NOT NULL | Data de início |
| `end_date` | DATE | NOT NULL | Data de término |
| `status` | ENUM | NOT NULL | planning, active, on_hold, completed, cancelled |
| `weight` | DECIMAL(5,2) | NULL | Peso no BSC (%) |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |
| `updated_at` | TIMESTAMP | NOT NULL | Data de atualização |
| `deleted_at` | TIMESTAMP | NULL | Soft delete |

**Índices:**
- `idx_bsc_obj_tenant_code` (tenant_id, code) UNIQUE WHERE deleted_at IS NULL
- `idx_bsc_obj_perspective` (perspective_id)
- `idx_bsc_obj_unit` (organizational_unit_id)
- `idx_bsc_obj_owner` (owner_id)

**Constraints:**
- FK `tenant_id` → `tenants(id)` ON DELETE CASCADE
- FK `perspective_id` → `bsc_perspectives(id)` ON DELETE RESTRICT
- FK `organizational_unit_id` → `organizational_units(id)` ON DELETE SET NULL
- FK `owner_id` → `persons(id)` ON DELETE SET NULL

---

#### Tabela: `bsc_indicators`

Indicadores (KPIs) do BSC.

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `objective_id` | UUID | FK, NOT NULL | Referência ao objetivo |
| `code` | VARCHAR(50) | NOT NULL | Código do indicador |
| `name` | VARCHAR(200) | NOT NULL | Nome do indicador |
| `description` | TEXT | NULL | Descrição |
| `formula` | TEXT | NULL | Fórmula de cálculo |
| `unit_of_measure` | VARCHAR(50) | NULL | Unidade (%, R$, unidades) |
| `baseline_value` | DECIMAL(15,2) | NULL | Valor baseline |
| `target_value` | DECIMAL(15,2) | NOT NULL | Meta |
| `current_value` | DECIMAL(15,2) | NULL | Valor atual |
| `measurement_frequency` | ENUM | NOT NULL | daily, weekly, monthly, quarterly, yearly |
| `data_source` | VARCHAR(200) | NULL | Fonte dos dados |
| `is_higher_better` | BOOLEAN | NOT NULL, DEFAULT true | Se maior é melhor |
| `status` | ENUM | NOT NULL | on_track, at_risk, off_track, achieved |
| `last_measured_date` | DATE | NULL | Data da última medição |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |
| `updated_at` | TIMESTAMP | NOT NULL | Data de atualização |
| `deleted_at` | TIMESTAMP | NULL | Soft delete |

**Índices:**
- `idx_bsc_ind_objective` (objective_id)
- `idx_bsc_ind_status` (status)

**Constraints:**
- FK `objective_id` → `bsc_objectives(id)` ON DELETE CASCADE

---

#### Tabela: `bsc_indicator_measurements`

Histórico de medições dos indicadores.

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `indicator_id` | UUID | FK, NOT NULL | Referência ao indicador |
| `measurement_date` | DATE | NOT NULL | Data da medição |
| `value` | DECIMAL(15,2) | NOT NULL | Valor medido |
| `notes` | TEXT | NULL | Observações |
| `measured_by` | UUID | FK, NULL | Quem mediu |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |

**Índices:**
- `idx_bsc_meas_indicator` (indicator_id)
- `idx_bsc_meas_date` (measurement_date)
- `idx_bsc_meas_unique` (indicator_id, measurement_date) UNIQUE

**Constraints:**
- FK `indicator_id` → `bsc_indicators(id)` ON DELETE CASCADE
- FK `measured_by` → `persons(id)` ON DELETE SET NULL

---

### 2.7 Domínio: PDI (Plano de Desenvolvimento Individual)

#### Tabela: `pdi_cycles`

Ciclos de PDI (semestral, anual, etc).

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `person_id` | UUID | FK, NOT NULL | Pessoa do PDI |
| `coach_id` | UUID | FK, NULL | Coach/BP responsável |
| `manager_id` | UUID | FK, NULL | Gestor da pessoa |
| `cycle_name` | VARCHAR(100) | NOT NULL | Nome do ciclo (ex: "2024 S1") |
| `start_date` | DATE | NOT NULL | Data de início |
| `end_date` | DATE | NOT NULL | Data de término |
| `status` | ENUM | NOT NULL | draft, active, on_hold, completed, cancelled |
| `overall_progress` | DECIMAL(5,2) | NULL, DEFAULT 0 | Progresso geral (%) |
| `focus_areas` | JSONB | NULL | Áreas de foco prioritárias |
| `context` | TEXT | NULL | Contexto do PDI |
| `kickoff_date` | DATE | NULL | Data do kickoff |
| `completion_date` | DATE | NULL | Data de conclusão |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |
| `updated_at` | TIMESTAMP | NOT NULL | Data de atualização |
| `deleted_at` | TIMESTAMP | NULL | Soft delete |

**Índices:**
- `idx_pdi_cycle_person` (person_id)
- `idx_pdi_cycle_coach` (coach_id)
- `idx_pdi_cycle_status` (status)
- `idx_pdi_cycle_dates` (start_date, end_date)

**Constraints:**
- FK `person_id` → `persons(id)` ON DELETE CASCADE
- FK `coach_id` → `persons(id)` ON DELETE SET NULL
- FK `manager_id` → `persons(id)` ON DELETE SET NULL
- `chk_dates`: end_date > start_date

---

#### Tabela: `pdi_goals`

Metas de desenvolvimento dentro do ciclo PDI.

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `pdi_cycle_id` | UUID | FK, NOT NULL | Referência ao ciclo PDI |
| `competency_id` | UUID | FK, NULL | Competência relacionada |
| `title` | VARCHAR(200) | NOT NULL | Título da meta |
| `description` | TEXT | NULL | Descrição detalhada |
| `goal_type` | ENUM | NOT NULL | competency, behavior, project, learning |
| `current_level` | INTEGER | NULL | Nível atual (1-5) |
| `target_level` | INTEGER | NULL | Nível esperado (1-5) |
| `priority` | ENUM | NOT NULL | high, medium, low |
| `status` | ENUM | NOT NULL | not_started, in_progress, blocked, completed, cancelled |
| `progress` | DECIMAL(5,2) | NULL, DEFAULT 0 | Progresso (%) |
| `deadline` | DATE | NULL | Prazo |
| `completed_date` | DATE | NULL | Data de conclusão |
| `success_criteria` | TEXT | NULL | Critérios de sucesso |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |
| `updated_at` | TIMESTAMP | NOT NULL | Data de atualização |
| `deleted_at` | TIMESTAMP | NULL | Soft delete |

**Índices:**
- `idx_pdi_goal_cycle` (pdi_cycle_id)
- `idx_pdi_goal_competency` (competency_id)
- `idx_pdi_goal_status` (status)
- `idx_pdi_goal_deadline` (deadline)

**Constraints:**
- FK `pdi_cycle_id` → `pdi_cycles(id)` ON DELETE CASCADE
- FK `competency_id` → `competencies(id)` ON DELETE SET NULL
- `chk_levels`: current_level BETWEEN 1 AND 5, target_level BETWEEN 1 AND 5
- `chk_progress`: progress BETWEEN 0 AND 100

---

#### Tabela: `pdi_actions`

Ações específicas para atingir as metas do PDI.

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `pdi_goal_id` | UUID | FK, NOT NULL | Referência à meta PDI |
| `title` | VARCHAR(200) | NOT NULL | Título da ação |
| `description` | TEXT | NULL | Descrição da ação |
| `action_type` | ENUM | NOT NULL | course, reading, mentoring, project, practice, other |
| `estimated_hours` | DECIMAL(6,2) | NULL | Horas estimadas |
| `actual_hours` | DECIMAL(6,2) | NULL | Horas realizadas |
| `due_date` | DATE | NULL | Data prevista |
| `completed_date` | DATE | NULL | Data de conclusão |
| `status` | ENUM | NOT NULL | pending, in_progress, completed, cancelled |
| `resources_needed` | TEXT | NULL | Recursos necessários |
| `evidence_url` | VARCHAR(500) | NULL | URL da evidência |
| `notes` | TEXT | NULL | Observações |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |
| `updated_at` | TIMESTAMP | NOT NULL | Data de atualização |
| `deleted_at` | TIMESTAMP | NULL | Soft delete |

**Índices:**
- `idx_pdi_action_goal` (pdi_goal_id)
- `idx_pdi_action_status` (status)
- `idx_pdi_action_due` (due_date)

**Constraints:**
- FK `pdi_goal_id` → `pdi_goals(id)` ON DELETE CASCADE

---

#### Tabela: `pdi_checkpoints`

Checkpoints de acompanhamento do PDI (semanal/quinzenal).

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `pdi_cycle_id` | UUID | FK, NOT NULL | Referência ao ciclo PDI |
| `checkpoint_date` | DATE | NOT NULL | Data do checkpoint |
| `type` | ENUM | NOT NULL | weekly, biweekly, monthly, milestone |
| `status` | ENUM | NOT NULL | scheduled, completed, cancelled |
| `overall_sentiment` | ENUM | NULL | very_negative, negative, neutral, positive, very_positive |
| `progress_summary` | TEXT | NULL | Resumo do progresso |
| `wins` | TEXT | NULL | Vitórias da semana/período |
| `challenges` | TEXT | NULL | Desafios enfrentados |
| `next_steps` | TEXT | NULL | Próximos passos |
| `coach_notes` | TEXT | NULL | Notas do coach |
| `completed_by` | UUID | FK, NULL | Quem completou |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |
| `updated_at` | TIMESTAMP | NOT NULL | Data de atualização |

**Índices:**
- `idx_pdi_check_cycle` (pdi_cycle_id)
- `idx_pdi_check_date` (checkpoint_date)

**Constraints:**
- FK `pdi_cycle_id` → `pdi_cycles(id)` ON DELETE CASCADE
- FK `completed_by` → `persons(id)` ON DELETE SET NULL

---

### 2.8 Domínio: Feedback e 1:1

#### Tabela: `one_on_ones`

Reuniões 1:1 entre gestor e liderado.

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `manager_id` | UUID | FK, NOT NULL | Gestor |
| `person_id` | UUID | FK, NOT NULL | Liderado |
| `scheduled_date` | TIMESTAMP | NOT NULL | Data/hora agendada |
| `actual_date` | TIMESTAMP | NULL | Data/hora realizada |
| `duration_minutes` | INTEGER | NULL | Duração em minutos |
| `status` | ENUM | NOT NULL | scheduled, completed, cancelled, rescheduled |
| `agenda` | JSONB | NULL | Pauta estruturada |
| `notes` | TEXT | NULL | Anotações da reunião |
| `mood_check` | ENUM | NULL | very_low, low, neutral, high, very_high |
| `energy_level` | INTEGER | NULL | 1-10 |
| `follow_up_items` | JSONB | NULL | Itens de acompanhamento |
| `ai_suggestions` | JSONB | NULL | Sugestões da IA |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |
| `updated_at` | TIMESTAMP | NOT NULL | Data de atualização |
| `deleted_at` | TIMESTAMP | NULL | Soft delete |

**Índices:**
- `idx_one_on_one_manager` (manager_id)
- `idx_one_on_one_person` (person_id)
- `idx_one_on_one_scheduled` (scheduled_date)
- `idx_one_on_one_status` (status)

**Constraints:**
- FK `manager_id` → `persons(id)` ON DELETE CASCADE
- FK `person_id` → `persons(id)` ON DELETE CASCADE

---

#### Tabela: `feedbacks`

Feedbacks (contínuos, estruturados, 360, etc).

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `from_person_id` | UUID | FK, NOT NULL | Quem deu o feedback |
| `to_person_id` | UUID | FK, NOT NULL | Quem recebeu |
| `one_on_one_id` | UUID | FK, NULL | Associado a 1:1 (opcional) |
| `feedback_type` | ENUM | NOT NULL | continuous, formal, peer, upward, 360 |
| `context` | VARCHAR(200) | NULL | Contexto/situação |
| `content` | TEXT | NOT NULL | Conteúdo do feedback |
| `sentiment` | ENUM | NULL | positive, constructive, developmental |
| `competency_id` | UUID | FK, NULL | Competência relacionada |
| `is_private` | BOOLEAN | NOT NULL, DEFAULT false | Se é privado |
| `visibility` | ENUM | NOT NULL | private, manager_only, person_and_manager, public |
| `delivered_date` | TIMESTAMP | NULL | Quando foi entregue |
| `status` | ENUM | NOT NULL | draft, delivered, acknowledged, archived |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |
| `updated_at` | TIMESTAMP | NOT NULL | Data de atualização |
| `deleted_at` | TIMESTAMP | NULL | Soft delete |

**Índices:**
- `idx_feedback_from` (from_person_id)
- `idx_feedback_to` (to_person_id)
- `idx_feedback_type` (feedback_type)
- `idx_feedback_delivered` (delivered_date)

**Constraints:**
- FK `from_person_id` → `persons(id)` ON DELETE CASCADE
- FK `to_person_id` → `persons(id)` ON DELETE CASCADE
- FK `one_on_one_id` → `one_on_ones(id)` ON DELETE SET NULL
- FK `competency_id` → `competencies(id)` ON DELETE SET NULL

---

### 2.9 Domínio: Recomendações IA

#### Tabela: `ai_recommendations`

Recomendações geradas pela IA.

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `tenant_id` | UUID | FK, NOT NULL | Referência ao tenant |
| `person_id` | UUID | FK, NULL | Pessoa relacionada (se aplicável) |
| `context_type` | ENUM | NOT NULL | pdi, one_on_one, feedback, pulse, gap_analysis |
| `context_id` | UUID | NULL | ID do contexto (PDI, 1:1, etc) |
| `recommendation_type` | ENUM | NOT NULL | conversation_topic, action_item, alert, learning_path |
| `priority` | ENUM | NOT NULL | low, medium, high, urgent |
| `title` | VARCHAR(200) | NOT NULL | Título da recomendação |
| `description` | TEXT | NOT NULL | Descrição detalhada |
| `rationale` | TEXT | NULL | Justificativa (explicabilidade) |
| `suggested_actions` | JSONB | NULL | Ações sugeridas |
| `model_used` | VARCHAR(100) | NULL | Modelo IA utilizado |
| `confidence_score` | DECIMAL(5,2) | NULL | Score de confiança (0-100) |
| `status` | ENUM | NOT NULL | pending, accepted, rejected, implemented, dismissed |
| `reviewed_by` | UUID | FK, NULL | Quem revisou |
| `reviewed_at` | TIMESTAMP | NULL | Quando foi revisado |
| `created_at` | TIMESTAMP | NOT NULL | Data de criação |
| `updated_at` | TIMESTAMP | NOT NULL | Data de atualização |

**Índices:**
- `idx_ai_rec_tenant` (tenant_id)
- `idx_ai_rec_person` (person_id)
- `idx_ai_rec_context` (context_type, context_id)
- `idx_ai_rec_status` (status)
- `idx_ai_rec_priority` (priority)

**Constraints:**
- FK `tenant_id` → `tenants(id)` ON DELETE CASCADE
- FK `person_id` → `persons(id)` ON DELETE CASCADE
- FK `reviewed_by` → `persons(id)` ON DELETE SET NULL

---

### 2.10 Domínio: Auditoria

#### Tabela: `audit_logs`

Log completo de auditoria de todas operações sensíveis.

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | UUID | PK, NOT NULL | Identificador único |
| `tenant_id` | UUID | FK, NOT NULL | Referência ao tenant |
| `user_id` | UUID | FK, NULL | Usuário que executou ação |
| `action` | VARCHAR(100) | NOT NULL | Tipo de ação (create, update, delete) |
| `entity_type` | VARCHAR(100) | NOT NULL | Tipo da entidade (Person, PDI, etc) |
| `entity_id` | UUID | NOT NULL | ID da entidade afetada |
| `changes` | JSONB | NULL | Mudanças realizadas (before/after) |
| `ip_address` | VARCHAR(45) | NULL | IP de origem |
| `user_agent` | VARCHAR(500) | NULL | User agent |
| `status` | ENUM | NOT NULL | success, failure, partial |
| `error_message` | TEXT | NULL | Mensagem de erro (se houver) |
| `metadata` | JSONB | NULL | Metadados adicionais |
| `timestamp` | TIMESTAMP | NOT NULL | Data/hora da ação |

**Índices:**
- `idx_audit_tenant` (tenant_id)
- `idx_audit_user` (user_id)
- `idx_audit_entity` (entity_type, entity_id)
- `idx_audit_timestamp` (timestamp)
- `idx_audit_action` (action)

**Constraints:**
- FK `tenant_id` → `tenants(id)` ON DELETE CASCADE
- FK `user_id` → `persons(id)` ON DELETE SET NULL

**Particionamento:**
- Considerar particionamento por mês (`timestamp`) para melhor performance

---

## 3. Domínios e Enumerações

### 3.1 Status e Estados

```sql
-- Status de Tenant
ENUM tenant_status: 'active', 'suspended', 'inactive'

-- Planos de assinatura
ENUM subscription_tier: 'free', 'basic', 'professional', 'enterprise'

-- Status de emprego
ENUM employment_status: 'active', 'on_leave', 'terminated'

-- Tipos de unidade organizacional
ENUM organizational_unit_type: 'department', 'team', 'area', 'division'

-- Estilos DISC
ENUM disc_style: 'D', 'I', 'S', 'C'

-- Tipos de avaliação organizacional
ENUM assessment_type: 'collective_listening', 'climate_survey', 'maturity'

-- Status de avaliação
ENUM assessment_status: 'draft', 'active', 'closed', 'archived'

-- Tipos de pesquisa de pulso
ENUM pulse_type: 'mood', 'energy', 'engagement', 'risk'

-- Frequência
ENUM frequency: 'daily', 'weekly', 'biweekly', 'monthly', 'quarterly', 'yearly'

-- Sentimento geral
ENUM sentiment: 'very_negative', 'negative', 'neutral', 'positive', 'very_positive'

-- Categoria de competência
ENUM competency_category: 'technical', 'behavioral', 'leadership', 'core'

-- Tipo de nível de competência
ENUM level_type: 'numeric', 'descriptive', 'boolean'

-- Status de objetivo BSC
ENUM bsc_objective_status: 'planning', 'active', 'on_hold', 'completed', 'cancelled'

-- Status de indicador
ENUM indicator_status: 'on_track', 'at_risk', 'off_track', 'achieved'

-- Status de ciclo PDI
ENUM pdi_cycle_status: 'draft', 'active', 'on_hold', 'completed', 'cancelled'

-- Tipos de meta PDI
ENUM pdi_goal_type: 'competency', 'behavior', 'project', 'learning'

-- Prioridade
ENUM priority: 'high', 'medium', 'low'

-- Status de meta PDI
ENUM pdi_goal_status: 'not_started', 'in_progress', 'blocked', 'completed', 'cancelled'

-- Tipos de ação PDI
ENUM pdi_action_type: 'course', 'reading', 'mentoring', 'project', 'practice', 'other'

-- Status de ação
ENUM action_status: 'pending', 'in_progress', 'completed', 'cancelled'

-- Tipos de checkpoint
ENUM checkpoint_type: 'weekly', 'biweekly', 'monthly', 'milestone'

-- Status de checkpoint
ENUM checkpoint_status: 'scheduled', 'completed', 'cancelled'

-- Status de 1:1
ENUM one_on_one_status: 'scheduled', 'completed', 'cancelled', 'rescheduled'

-- Tipos de feedback
ENUM feedback_type: 'continuous', 'formal', 'peer', 'upward', '360'

-- Sentimento de feedback
ENUM feedback_sentiment: 'positive', 'constructive', 'developmental'

-- Visibilidade de feedback
ENUM feedback_visibility: 'private', 'manager_only', 'person_and_manager', 'public'

-- Status de feedback
ENUM feedback_status: 'draft', 'delivered', 'acknowledged', 'archived'

-- Tipos de contexto para IA
ENUM ai_context_type: 'pdi', 'one_on_one', 'feedback', 'pulse', 'gap_analysis'

-- Tipos de recomendação IA
ENUM ai_recommendation_type: 'conversation_topic', 'action_item', 'alert', 'learning_path'

-- Status de recomendação
ENUM ai_recommendation_status: 'pending', 'accepted', 'rejected', 'implemented', 'dismissed'

-- Status de auditoria
ENUM audit_status: 'success', 'failure', 'partial'
```

---

## 4. Estratégia Multi-Tenant

### 4.1 Princípios de Isolamento

Todos os dados são isolados por `tenant_id`, garantindo:

1. **Isolamento Físico**: Cada query SEMPRE filtra por `tenant_id`
2. **Segurança**: Impossível acessar dados de outro tenant
3. **Performance**: Índices compostos com tenant_id como primeiro campo

### 4.2 Row-Level Security (RLS)

Para bancos como PostgreSQL, implementar RLS:

```sql
-- Exemplo de política RLS
ALTER TABLE persons ENABLE ROW LEVEL SECURITY;

CREATE POLICY tenant_isolation ON persons
  USING (tenant_id = current_setting('app.current_tenant')::uuid);
```

### 4.3 Tenant Resolution

```
Request → Middleware → JWT/Token → Extract tenant_id → Set session context
```

### 4.4 Filtros Globais (Entity Framework Core)

```csharp
// Global query filter
modelBuilder.Entity<Person>()
    .HasQueryFilter(p => p.TenantId == _currentTenantId);
```

### 4.5 Particionamento (Opcional para Escala)

Para tenants muito grandes, considerar particionamento:

```sql
-- Particionamento por tenant_id (PostgreSQL)
CREATE TABLE audit_logs (
  ...
) PARTITION BY HASH (tenant_id);
```

---

## 5. Auditoria e Tracking

### 5.1 Campos Padrão de Auditoria

Todas as tabelas principais possuem:

```sql
created_at    TIMESTAMP NOT NULL DEFAULT NOW()
updated_at    TIMESTAMP NOT NULL DEFAULT NOW()
deleted_at    TIMESTAMP NULL  -- Soft delete
```

### 5.2 Triggers de Auditoria

```sql
-- Trigger para atualizar updated_at
CREATE TRIGGER update_timestamp
  BEFORE UPDATE ON <table>
  FOR EACH ROW
  EXECUTE FUNCTION update_updated_at_column();
```

### 5.3 Auditoria de Mudanças

A tabela `audit_logs` captura:

- **O quê**: entity_type, entity_id, action
- **Quem**: user_id, ip_address, user_agent
- **Quando**: timestamp
- **Detalhes**: changes (JSONB com before/after)

### 5.4 Dados Sensíveis

Campos sensíveis devem ter:
- Opt-in explícito
- Trilha de auditoria
- Possibilidade de anonimização/exclusão (LGPD)

---

## 6. Considerações de Performance

### 6.1 Índices Estratégicos

**Índices Compostos:**
```sql
-- Sempre tenant_id primeiro
CREATE INDEX idx_persons_tenant_email ON persons (tenant_id, email);
CREATE INDEX idx_pdi_cycles_tenant_status ON pdi_cycles (tenant_id, status);
```

**Índices para Queries Frequentes:**
```sql
-- Busca de PDIs ativos
CREATE INDEX idx_pdi_active ON pdi_cycles (person_id, status)
  WHERE status = 'active';

-- Gaps de competência
CREATE INDEX idx_competency_gaps ON person_competencies (person_id)
  WHERE target_level > current_level;
```

### 6.2 Desnormalização Estratégica

**Campos Calculados:**
- `overall_progress` em `pdi_cycles`
- `current_value` em `bsc_indicators`
- `participation_rate` em `organizational_assessments`

**Justificativa:** Evitar cálculos complexos em tempo real, especialmente em dashboards.

**Manutenção:** Via triggers ou jobs agendados.

### 6.3 JSONB vs Tabelas Normalizadas

**Usar JSONB para:**
- Dados de configuração (`settings`)
- Dados flexíveis (`metadata`, `permissions`)
- Arrays simples (`strengths`, `development_areas`)

**Usar Tabelas para:**
- Dados relacionais com queries complexas
- Dados que precisam de integridade referencial
- Dados frequentemente filtrados/ordenados

### 6.4 Particionamento

**Candidatos:**
- `audit_logs` (por mês/trimestre)
- `pulse_responses` (por data)
- `bsc_indicator_measurements` (por ano)

### 6.5 Arquivamento

Estratégia para dados antigos:
- Soft delete com `deleted_at`
- Tabelas de arquivo (`_archive`)
- Políticas de retenção por tipo de dado

---

## 7. Integridade Referencial

### 7.1 Cascata de Exclusão

**CASCADE:**
- `tenant_id` → Todas as tabelas (excluir tenant remove tudo)
- `pdi_cycle_id` → `pdi_goals`, `pdi_actions`, `pdi_checkpoints`
- `objective_id` → `bsc_indicators`

**SET NULL:**
- `manager_id`, `coach_id` (preservar estrutura se pessoa sair)
- `parent_id` em hierarquias

**RESTRICT:**
- `perspective_id` em `bsc_objectives` (não pode excluir perspectiva em uso)

### 7.2 Constraints de Domínio

```sql
-- Scores DISC
CONSTRAINT chk_disc_scores CHECK (
  d_score BETWEEN 0 AND 100 AND
  i_score BETWEEN 0 AND 100 AND
  s_score BETWEEN 0 AND 100 AND
  c_score BETWEEN 0 AND 100
)

-- Níveis de competência
CONSTRAINT chk_competency_levels CHECK (
  current_level BETWEEN 1 AND 5 AND
  (target_level IS NULL OR target_level BETWEEN 1 AND 5)
)

-- Progresso
CONSTRAINT chk_progress CHECK (progress BETWEEN 0 AND 100)

-- Datas lógicas
CONSTRAINT chk_dates CHECK (end_date >= start_date)
```

---

## 8. Decisões Arquiteturais

### 8.1 Soft Delete vs Hard Delete

**Decisão:** Usar soft delete (`deleted_at`) para todas entidades principais.

**Razão:**
- Preservação de histórico
- Auditoria completa
- Recuperação de dados
- Integridade referencial

**Exceção:** Logs e dados de telemetria podem usar hard delete.

### 8.2 UUIDs vs Auto-increment IDs

**Decisão:** Usar UUIDs para todas PKs.

**Razão:**
- Multi-tenant seguro (IDs não sequenciais)
- Facilita merge de dados
- Geração distribuída
- Segurança por obscuridade adicional

### 8.3 JSONB para Flexibilidade

**Decisão:** Usar JSONB para dados semi-estruturados.

**Razão:**
- Schema flexível onde necessário
- Menos migrações
- Performance adequada com índices GIN

**Cuidado:**
- Não abusar (manter normalização quando faz sentido)
- Documentar estrutura esperada

### 8.4 Tabelas de Histórico vs Eventos

**Decisão:** Usar tabelas de histórico diretas (ex: `bsc_indicator_measurements`).

**Razão:**
- Queries mais simples
- Performance previsível
- Menos complexidade inicial

**Evolução:** Considerar Event Sourcing se necessário no futuro.

### 8.5 Permissões em JSONB vs Tabelas

**Decisão:** Armazenar permissões em JSONB na tabela `roles`.

**Razão:**
- Flexibilidade para diferentes tenants
- Menos joins
- Evolução rápida

**Alternativa:** Migrar para tabela normalizada se permissões ficarem muito complexas.

---

## 9. Diagrama de Dependências

```
tenants
  └─ organizational_units
      ├─ persons
      │   ├─ disc_profiles
      │   ├─ person_competencies
      │   ├─ pdi_cycles
      │   │   ├─ pdi_goals
      │   │   │   └─ pdi_actions
      │   │   └─ pdi_checkpoints
      │   ├─ one_on_ones
      │   ├─ feedbacks
      │   └─ pulse_responses
      ├─ roles
      │   ├─ person_roles
      │   └─ role_competencies
      ├─ competencies
      ├─ bsc_perspectives
      │   └─ bsc_objectives
      │       └─ bsc_indicators
      │           └─ bsc_indicator_measurements
      ├─ organizational_assessments
      ├─ pulse_surveys
      │   └─ pulse_responses
      ├─ ai_recommendations
      └─ audit_logs
```

---

## 10. Próximos Passos

### 10.1 Validações Necessárias

- [ ] Revisar modelo com stakeholders técnicos
- [ ] Validar requisitos de LGPD/GDPR
- [ ] Definir políticas de retenção de dados
- [ ] Avaliar necessidade de criptografia em repouso

### 10.2 Implementação

1. **Fase 1**: Core (Tenants, Persons, Roles)
2. **Fase 2**: Assessments (DISC, EC)
3. **Fase 3**: PDI e Competências
4. **Fase 4**: BSC e Indicadores
5. **Fase 5**: Feedback e 1:1
6. **Fase 6**: IA e Recomendações

### 10.3 Migrações

- Usar Entity Framework Core Migrations
- Scripts SQL versionados
- Estratégia de rollback
- Testes de migração em staging

---

## Apêndice: SQL de Criação (Exemplo - PostgreSQL)

```sql
-- Exemplo de criação da tabela tenants
CREATE TABLE tenants (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  slug VARCHAR(100) UNIQUE NOT NULL,
  name VARCHAR(200) NOT NULL,
  status VARCHAR(20) NOT NULL CHECK (status IN ('active', 'suspended', 'inactive')),
  subscription_tier VARCHAR(20) NOT NULL CHECK (subscription_tier IN ('free', 'basic', 'professional', 'enterprise')),
  max_users INTEGER NOT NULL CHECK (max_users > 0),
  settings JSONB,
  created_at TIMESTAMP NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMP NOT NULL DEFAULT NOW(),
  deleted_at TIMESTAMP
);

CREATE INDEX idx_tenants_slug ON tenants (slug);
CREATE INDEX idx_tenants_status ON tenants (status);

-- Trigger para updated_at
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
  NEW.updated_at = NOW();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_tenants_updated_at
  BEFORE UPDATE ON tenants
  FOR EACH ROW
  EXECUTE FUNCTION update_updated_at_column();
```

---

**Documento Versão:** 1.0
**Data:** 2026-04-13
**Autor:** Equipe MCP RH
**Status:** Proposta Inicial
