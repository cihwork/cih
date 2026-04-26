# Especificação de APIs REST - MCP-RH

**Versão:** 1.0.0
**Data:** 2026-04-13
**Formato:** OpenAPI 3.0
**Base URL:** `https://api.mcp-rh.com/api/v1`

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Autenticação e Autorização](#autenticação-e-autorização)
3. [Multi-Tenancy](#multi-tenancy)
4. [Rate Limiting](#rate-limiting)
5. [Versionamento](#versionamento)
6. [Endpoints por Módulo](#endpoints-por-módulo)
   - [Identity & Tenant](#identity--tenant)
   - [People (Colaboradores)](#people-colaboradores)
   - [DISC (Assessments)](#disc-assessments)
   - [PDI (Planos de Desenvolvimento)](#pdi-planos-de-desenvolvimento)
   - [Performance (BSC e Metas)](#performance-bsc-e-metas)
   - [Engagement (Check-ins e Feedback)](#engagement-check-ins-e-feedback)
   - [AI Copilot](#ai-copilot)
7. [Schemas e Models](#schemas-e-models)
8. [Webhooks](#webhooks)
9. [Códigos de Erro](#códigos-de-erro)

---

## Visão Geral

A API MCP-RH segue os princípios REST e retorna respostas em formato JSON. Todas as requisições devem incluir headers de autenticação e identificação de tenant.

### Características

- **Protocolo:** HTTPS (TLS 1.2+)
- **Formato:** JSON (UTF-8)
- **Autenticação:** Bearer Token (JWT)
- **Multi-tenancy:** Header `X-Tenant-Id`
- **Versionamento:** URL path (`/api/v1`)
- **Rate Limiting:** Token bucket (varia por plano)

---

## Autenticação e Autorização

### Bearer Token (JWT)

Todas as requisições autenticadas devem incluir um token JWT no header `Authorization`.

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Estrutura do Token JWT

```json
{
  "sub": "user-uuid",
  "email": "usuario@empresa.com",
  "tenant_id": "tenant-uuid",
  "roles": ["Leader", "BP"],
  "permissions": ["pdi:read", "pdi:write", "disc:read"],
  "exp": 1735689600,
  "iat": 1735686000
}
```

### Roles e Permissions

| Role | Descrição | Scopes Típicos |
|------|-----------|----------------|
| **Admin** | Administrador do tenant | `*:*` (todos) |
| **BP** | Business Partner de RH | `people:*, pdi:*, disc:*, performance:read, engagement:read` |
| **Coach** | Coach interno | `pdi:*, disc:read, engagement:read` |
| **Leader** | Líder de equipe | `people:read, pdi:read, pdi:write_team, engagement:*, performance:read` |
| **Employee** | Colaborador | `pdi:read_own, pdi:write_own, disc:read_own, engagement:write_own` |

### Endpoint de Autenticação

**POST /auth/login**

```json
// Request
{
  "email": "usuario@empresa.com",
  "password": "senha_segura",
  "tenant_slug": "empresa-abc"
}

// Response 200 OK
{
  "access_token": "eyJhbGciOiJIUzI1NiIs...",
  "refresh_token": "dGhpcyBpcyBhIHJlZnJl...",
  "token_type": "Bearer",
  "expires_in": 3600,
  "user": {
    "id": "usr-001",
    "name": "João Silva",
    "email": "usuario@empresa.com",
    "roles": ["Leader"]
  }
}

// Response 401 Unauthorized
{
  "error": {
    "code": "INVALID_CREDENTIALS",
    "message": "E-mail ou senha inválidos",
    "timestamp": "2026-04-13T10:30:00Z"
  }
}
```

**POST /auth/refresh**

```json
// Request
{
  "refresh_token": "dGhpcyBpcyBhIHJlZnJl..."
}

// Response 200 OK
{
  "access_token": "eyJhbGciOiJIUzI1NiIs...",
  "expires_in": 3600
}
```

**POST /auth/logout**

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...

// Response 204 No Content
```

---

## Multi-Tenancy

### Header Obrigatório

Todas as requisições após autenticação devem incluir o header `X-Tenant-Id`:

```http
X-Tenant-Id: tenant-abc-uuid
```

### Isolamento de Dados

- Cada tenant possui dados completamente isolados
- Queries são automaticamente filtradas por `tenant_id` (EF Core Global Query Filter)
- Tentativa de acesso a dados de outro tenant retorna **403 Forbidden**

### Exemplo de Erro Multi-Tenant

```json
// Response 403 Forbidden
{
  "error": {
    "code": "TENANT_MISMATCH",
    "message": "Você não tem permissão para acessar recursos deste tenant",
    "timestamp": "2026-04-13T10:30:00Z"
  }
}
```

---

## Rate Limiting

### Limites por Plano

| Plano | Requisições/Minuto | Burst |
|-------|-------------------|-------|
| **Free** | 60 | 10 |
| **Professional** | 300 | 50 |
| **Enterprise** | 1000 | 200 |

### Headers de Rate Limit

```http
X-RateLimit-Limit: 300
X-RateLimit-Remaining: 245
X-RateLimit-Reset: 1735689600
```

### Resposta de Rate Limit Excedido

```json
// Response 429 Too Many Requests
{
  "error": {
    "code": "RATE_LIMIT_EXCEEDED",
    "message": "Limite de requisições excedido. Tente novamente em 30 segundos",
    "retry_after": 30,
    "timestamp": "2026-04-13T10:30:00Z"
  }
}
```

---

## Versionamento

### Estratégia de Versionamento

- **Método:** URL Path (`/api/v1`, `/api/v2`)
- **Versão Atual:** `v1`
- **Suporte:** Cada versão é mantida por **12 meses** após release da próxima

### Header de Depreciação

```http
Deprecation: Sun, 01 Jan 2027 00:00:00 GMT
Sunset: Mon, 01 Jul 2027 00:00:00 GMT
Link: <https://api.mcp-rh.com/api/v2/employees>; rel="alternate"
```

---

## Endpoints por Módulo

---

## Identity & Tenant

### 1. Listar Tenants do Usuário

**GET /tenants**

Retorna todos os tenants aos quais o usuário autenticado tem acesso.

```http
GET /api/v1/tenants
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

**Response 200 OK**

```json
{
  "data": [
    {
      "id": "tenant-001",
      "name": "Empresa ABC Ltda",
      "slug": "empresa-abc",
      "settings": {
        "disc_enabled": true,
        "pdi_cycle": "quarterly",
        "bsc_cascading": "full"
      },
      "plan": "Professional",
      "created_at": "2025-01-15T08:00:00Z"
    }
  ],
  "meta": {
    "total": 1
  }
}
```

### 2. Obter Configurações do Tenant

**GET /tenants/{tenantId}/settings**

```http
GET /api/v1/tenants/tenant-001/settings
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
X-Tenant-Id: tenant-001
```

**Response 200 OK**

```json
{
  "disc_enabled": true,
  "pdi_cycle": "quarterly",
  "bsc_cascading": "full",
  "engagement_pulse_frequency": "weekly",
  "ai_copilot_enabled": true,
  "culture_maturity_level": "conscious",
  "custom_competencies": true
}
```

### 3. Atualizar Configurações do Tenant

**PATCH /tenants/{tenantId}/settings**

```json
// Request
{
  "pdi_cycle": "monthly",
  "ai_copilot_enabled": true
}

// Response 200 OK
{
  "message": "Configurações atualizadas com sucesso",
  "updated_at": "2026-04-13T10:35:00Z"
}
```

---

## People (Colaboradores)

### 4. Listar Colaboradores

**GET /employees**

```http
GET /api/v1/employees?department=Tech&status=active&page=1&limit=20
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
X-Tenant-Id: tenant-001
```

**Query Parameters**

| Parâmetro | Tipo | Descrição |
|-----------|------|-----------|
| `department` | string | Filtrar por departamento |
| `status` | enum | `active`, `inactive`, `on_leave` |
| `manager_id` | uuid | Filtrar por gestor |
| `search` | string | Busca por nome ou e-mail |
| `page` | int | Número da página (default: 1) |
| `limit` | int | Itens por página (default: 20, max: 100) |

**Response 200 OK**

```json
{
  "data": [
    {
      "id": "emp-001",
      "name": "Pedro Costa",
      "email": "pedro.costa@empresa.com",
      "role": "Desenvolvedor Sênior",
      "department": "Tecnologia",
      "manager": {
        "id": "emp-050",
        "name": "Maria Santos"
      },
      "hire_date": "2023-03-15",
      "status": "active",
      "disc_profile": "D",
      "pdi_status": "in_progress",
      "avatar_url": "https://cdn.mcp-rh.com/avatars/emp-001.jpg"
    }
  ],
  "meta": {
    "current_page": 1,
    "per_page": 20,
    "total": 45,
    "total_pages": 3
  },
  "links": {
    "first": "/api/v1/employees?page=1",
    "last": "/api/v1/employees?page=3",
    "prev": null,
    "next": "/api/v1/employees?page=2"
  }
}
```

### 5. Obter Colaborador por ID

**GET /employees/{employeeId}**

```http
GET /api/v1/employees/emp-001
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
X-Tenant-Id: tenant-001
```

**Response 200 OK**

```json
{
  "id": "emp-001",
  "name": "Pedro Costa",
  "email": "pedro.costa@empresa.com",
  "role": "Desenvolvedor Sênior",
  "department": "Tecnologia",
  "manager": {
    "id": "emp-050",
    "name": "Maria Santos",
    "email": "maria.santos@empresa.com"
  },
  "hire_date": "2023-03-15",
  "status": "active",
  "disc_profile": {
    "primary": "D",
    "secondary": "I",
    "assessment_date": "2025-06-10"
  },
  "pdi": {
    "current_cycle": "Q2-2026",
    "status": "in_progress",
    "progress": 65,
    "last_update": "2026-04-10T14:20:00Z"
  },
  "performance": {
    "bsc_score": 82,
    "goals_completed": 8,
    "goals_total": 12
  },
  "team_members": [],
  "created_at": "2023-03-15T08:00:00Z",
  "updated_at": "2026-04-13T09:15:00Z"
}
```

### 6. Criar Colaborador

**POST /employees**

```json
// Request
{
  "name": "Ana Silva",
  "email": "ana.silva@empresa.com",
  "role": "Analista de Marketing",
  "department": "Marketing",
  "manager_id": "emp-060",
  "hire_date": "2026-05-01",
  "status": "active"
}

// Response 201 Created
{
  "id": "emp-150",
  "name": "Ana Silva",
  "email": "ana.silva@empresa.com",
  "role": "Analista de Marketing",
  "department": "Marketing",
  "manager": {
    "id": "emp-060",
    "name": "Carlos Mendes"
  },
  "hire_date": "2026-05-01",
  "status": "active",
  "created_at": "2026-04-13T10:40:00Z"
}
```

### 7. Atualizar Colaborador

**PATCH /employees/{employeeId}**

```json
// Request
{
  "role": "Desenvolvedor Pleno",
  "department": "Tecnologia"
}

// Response 200 OK
{
  "id": "emp-001",
  "name": "Pedro Costa",
  "role": "Desenvolvedor Pleno",
  "department": "Tecnologia",
  "updated_at": "2026-04-13T10:45:00Z"
}
```

### 8. Obter Equipe de um Líder

**GET /employees/{employeeId}/team**

```http
GET /api/v1/employees/emp-050/team
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
X-Tenant-Id: tenant-001
```

**Response 200 OK**

```json
{
  "manager": {
    "id": "emp-050",
    "name": "Maria Santos",
    "role": "Tech Lead"
  },
  "team_members": [
    {
      "id": "emp-001",
      "name": "Pedro Costa",
      "role": "Desenvolvedor Sênior",
      "disc_profile": "D",
      "pdi_status": "in_progress",
      "health_score": "green"
    },
    {
      "id": "emp-002",
      "name": "Julia Fernandes",
      "role": "Desenvolvedora Júnior",
      "disc_profile": "S",
      "pdi_status": "active",
      "health_score": "yellow"
    }
  ],
  "meta": {
    "total_members": 5,
    "team_health": {
      "green": 3,
      "yellow": 1,
      "red": 1
    }
  }
}
```

---

## DISC (Assessments)

### 9. Listar Assessments DISC

**GET /disc/assessments**

```http
GET /api/v1/disc/assessments?employee_id=emp-001&status=completed
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
X-Tenant-Id: tenant-001
```

**Response 200 OK**

```json
{
  "data": [
    {
      "id": "disc-001",
      "employee_id": "emp-001",
      "employee_name": "Pedro Costa",
      "status": "completed",
      "primary_profile": "D",
      "secondary_profile": "I",
      "scores": {
        "D": 85,
        "I": 70,
        "S": 35,
        "C": 40
      },
      "completed_at": "2025-06-10T14:30:00Z",
      "report_url": "https://cdn.mcp-rh.com/reports/disc-001.pdf"
    }
  ],
  "meta": {
    "total": 1
  }
}
```

### 10. Criar Assessment DISC

**POST /disc/assessments**

```json
// Request
{
  "employee_id": "emp-150",
  "assessment_type": "full"
}

// Response 201 Created
{
  "id": "disc-150",
  "employee_id": "emp-150",
  "status": "pending",
  "assessment_link": "https://app.mcp-rh.com/disc/disc-150/take",
  "expires_at": "2026-04-20T23:59:59Z",
  "created_at": "2026-04-13T10:50:00Z"
}
```

### 11. Obter Resultado de Assessment DISC

**GET /disc/assessments/{assessmentId}**

```http
GET /api/v1/disc/assessments/disc-001
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
X-Tenant-Id: tenant-001
```

**Response 200 OK**

```json
{
  "id": "disc-001",
  "employee_id": "emp-001",
  "employee_name": "Pedro Costa",
  "status": "completed",
  "primary_profile": "D",
  "secondary_profile": "I",
  "scores": {
    "D": 85,
    "I": 70,
    "S": 35,
    "C": 40
  },
  "interpretation": {
    "strengths": [
      "Tomada de decisão rápida",
      "Orientação a resultados",
      "Iniciativa e proatividade"
    ],
    "development_areas": [
      "Paciência com processos detalhados",
      "Escuta ativa",
      "Delegação efetiva"
    ],
    "communication_style": "Direto, objetivo e assertivo. Prefere conversas rápidas e focadas em soluções.",
    "ideal_work_environment": "Ambiente dinâmico com autonomia para tomada de decisão."
  },
  "behavioral_context": {
    "in_pressure": "Tende a ser mais assertivo e direto, podendo parecer impaciente.",
    "in_collaboration": "Valoriza eficiência, pode ter dificuldade com processos longos de consenso.",
    "leadership_style": "Líder diretivo, foca em resultados e delega com clareza."
  },
  "completed_at": "2025-06-10T14:30:00Z",
  "report_url": "https://cdn.mcp-rh.com/reports/disc-001.pdf"
}
```

### 12. Obter Perfil DISC de Equipe

**GET /disc/team-profile**

```http
GET /api/v1/disc/team-profile?manager_id=emp-050
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
X-Tenant-Id: tenant-001
```

**Response 200 OK**

```json
{
  "manager": {
    "id": "emp-050",
    "name": "Maria Santos"
  },
  "team_profile": {
    "total_members": 5,
    "profile_distribution": {
      "D": 2,
      "I": 1,
      "S": 1,
      "C": 1
    },
    "dominant_profile": "D",
    "balance_score": 75,
    "friction_risks": [
      {
        "risk": "Excesso de perfis D pode gerar conflitos de decisão",
        "severity": "medium",
        "recommendation": "Estabelecer processos claros de tomada de decisão e ouvir vozes minoritárias (S e C)"
      }
    ],
    "collaboration_insights": [
      "Time com foco em resultados",
      "Pode precisar de suporte em processos detalhados",
      "Recomenda-se atenção à escuta ativa"
    ]
  }
}
```

---

## PDI (Planos de Desenvolvimento)

### 13. Listar PDIs

**GET /pdi**

```http
GET /api/v1/pdi?employee_id=emp-001&status=active&cycle=Q2-2026
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
X-Tenant-Id: tenant-001
```

**Query Parameters**

| Parâmetro | Tipo | Descrição |
|-----------|------|-----------|
| `employee_id` | uuid | Filtrar por colaborador |
| `status` | enum | `draft`, `active`, `completed`, `archived` |
| `cycle` | string | Ciclo (ex: `Q2-2026`) |
| `manager_id` | uuid | PDIs da equipe de um gestor |

**Response 200 OK**

```json
{
  "data": [
    {
      "id": "pdi-001",
      "employee_id": "emp-001",
      "employee_name": "Pedro Costa",
      "cycle": "Q2-2026",
      "status": "active",
      "progress": 65,
      "goals_count": 4,
      "goals_completed": 2,
      "created_at": "2026-04-01T08:00:00Z",
      "last_update": "2026-04-10T14:20:00Z"
    }
  ],
  "meta": {
    "total": 1
  }
}
```

### 14. Obter PDI por ID

**GET /pdi/{pdiId}**

```http
GET /api/v1/pdi/pdi-001
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
X-Tenant-Id: tenant-001
```

**Response 200 OK**

```json
{
  "id": "pdi-001",
  "employee": {
    "id": "emp-001",
    "name": "Pedro Costa",
    "role": "Desenvolvedor Sênior"
  },
  "cycle": "Q2-2026",
  "status": "active",
  "progress": 65,
  "goals": [
    {
      "id": "goal-001",
      "title": "Dominar arquitetura de microservices",
      "competency": {
        "id": "comp-tech-001",
        "name": "Arquitetura de Software",
        "category": "Técnica"
      },
      "current_level": 3,
      "target_level": 5,
      "status": "in_progress",
      "progress": 60,
      "actions": [
        {
          "id": "action-001",
          "description": "Ler livro 'Building Microservices' (Sam Newman)",
          "type": "70_on_job",
          "status": "completed",
          "completed_at": "2026-04-05T10:00:00Z"
        },
        {
          "id": "action-002",
          "description": "Refatorar módulo de pagamentos para microserviço",
          "type": "20_mentoring",
          "status": "in_progress",
          "progress": 40
        }
      ],
      "checkpoints": [
        {
          "id": "check-001",
          "date": "2026-04-10T14:00:00Z",
          "notes": "Progresso bom. Módulo de pagamentos 40% refatorado.",
          "blockers": "Necessário alinhamento com arquitetura sobre service mesh",
          "next_steps": "Finalizar refatoração e apresentar em tech talk"
        }
      ]
    }
  ],
  "created_at": "2026-04-01T08:00:00Z",
  "updated_at": "2026-04-10T14:20:00Z"
}
```

### 15. Criar PDI

**POST /pdi**

```json
// Request
{
  "employee_id": "emp-001",
  "cycle": "Q2-2026",
  "goals": [
    {
      "title": "Melhorar comunicação executiva",
      "competency_id": "comp-soft-005",
      "current_level": 2,
      "target_level": 4,
      "actions": [
        {
          "description": "Curso de apresentações executivas",
          "type": "10_formal"
        },
        {
          "description": "Apresentar resultados em reunião de diretoria",
          "type": "70_on_job"
        }
      ]
    }
  ]
}

// Response 201 Created
{
  "id": "pdi-150",
  "employee_id": "emp-001",
  "cycle": "Q2-2026",
  "status": "draft",
  "goals_count": 1,
  "created_at": "2026-04-13T11:00:00Z"
}
```

### 16. Atualizar PDI

**PATCH /pdi/{pdiId}**

```json
// Request
{
  "status": "active"
}

// Response 200 OK
{
  "id": "pdi-001",
  "status": "active",
  "updated_at": "2026-04-13T11:05:00Z"
}
```

### 17. Adicionar Checkpoint em Goal

**POST /pdi/{pdiId}/goals/{goalId}/checkpoints**

```json
// Request
{
  "notes": "Progresso excelente. Apresentação foi muito bem recebida pela diretoria.",
  "progress": 80,
  "blockers": null,
  "next_steps": "Repetir apresentação para outras áreas da empresa"
}

// Response 201 Created
{
  "id": "check-005",
  "date": "2026-04-13T11:10:00Z",
  "notes": "Progresso excelente. Apresentação foi muito bem recebida pela diretoria.",
  "progress": 80,
  "created_at": "2026-04-13T11:10:00Z"
}
```

### 18. Listar Competências (Biblioteca)

**GET /competencies**

```http
GET /api/v1/competencies?category=Técnica&search=arquitetura
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
X-Tenant-Id: tenant-001
```

**Response 200 OK**

```json
{
  "data": [
    {
      "id": "comp-tech-001",
      "name": "Arquitetura de Software",
      "category": "Técnica",
      "description": "Capacidade de projetar e implementar arquiteturas de software escaláveis e manuteníveis",
      "levels": [
        {
          "level": 1,
          "description": "Compreende conceitos básicos de arquitetura"
        },
        {
          "level": 2,
          "description": "Implementa componentes seguindo arquitetura definida"
        },
        {
          "level": 3,
          "description": "Desenha módulos e define interfaces"
        },
        {
          "level": 4,
          "description": "Projeta arquitetura de sistemas complexos"
        },
        {
          "level": 5,
          "description": "Define diretrizes de arquitetura para toda a organização"
        }
      ],
      "is_custom": false
    }
  ],
  "meta": {
    "total": 15
  }
}
```

---

## Performance (BSC e Metas)

### 19. Listar Metas BSC

**GET /performance/goals**

```http
GET /api/v1/performance/goals?employee_id=emp-001&period=Q2-2026&status=active
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
X-Tenant-Id: tenant-001
```

**Response 200 OK**

```json
{
  "data": [
    {
      "id": "goal-bsc-001",
      "title": "Reduzir tempo de deploy em 50%",
      "description": "Automatizar pipeline de CI/CD para reduzir tempo médio de deploy de 2h para 1h",
      "owner_id": "emp-001",
      "owner_name": "Pedro Costa",
      "bsc_perspective": "Processos Internos",
      "metric": {
        "name": "Tempo médio de deploy",
        "unit": "minutos",
        "baseline": 120,
        "target": 60,
        "current": 85
      },
      "progress": 58,
      "status": "on_track",
      "period": "Q2-2026",
      "cascaded_from": {
        "id": "goal-bsc-corp-015",
        "title": "Aumentar eficiência operacional em 30%",
        "owner": "CTO"
      },
      "due_date": "2026-06-30",
      "created_at": "2026-04-01T08:00:00Z"
    }
  ],
  "meta": {
    "total": 8
  }
}
```

### 20. Criar Meta BSC

**POST /performance/goals**

```json
// Request
{
  "title": "Aumentar NPS do produto em 15 pontos",
  "description": "Melhorar experiência do usuário para aumentar NPS de 45 para 60",
  "owner_id": "emp-060",
  "bsc_perspective": "Clientes",
  "metric": {
    "name": "NPS (Net Promoter Score)",
    "unit": "pontos",
    "baseline": 45,
    "target": 60
  },
  "period": "Q2-2026",
  "due_date": "2026-06-30"
}

// Response 201 Created
{
  "id": "goal-bsc-150",
  "title": "Aumentar NPS do produto em 15 pontos",
  "status": "active",
  "progress": 0,
  "created_at": "2026-04-13T11:20:00Z"
}
```

### 21. Atualizar Progresso de Meta

**PATCH /performance/goals/{goalId}/progress**

```json
// Request
{
  "current_value": 52,
  "notes": "Implementamos novo onboarding. NPS subiu de 45 para 52."
}

// Response 200 OK
{
  "id": "goal-bsc-150",
  "metric": {
    "current": 52,
    "target": 60
  },
  "progress": 47,
  "status": "at_risk",
  "updated_at": "2026-04-13T11:25:00Z"
}
```

### 22. Obter Dashboard BSC (Scorecard)

**GET /performance/scorecard**

```http
GET /api/v1/performance/scorecard?employee_id=emp-001&period=Q2-2026
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
X-Tenant-Id: tenant-001
```

**Response 200 OK**

```json
{
  "employee": {
    "id": "emp-001",
    "name": "Pedro Costa"
  },
  "period": "Q2-2026",
  "overall_score": 82,
  "perspectives": [
    {
      "name": "Processos Internos",
      "weight": 40,
      "score": 85,
      "goals_count": 3,
      "goals_completed": 1,
      "goals_on_track": 2
    },
    {
      "name": "Aprendizado e Crescimento",
      "weight": 30,
      "score": 75,
      "goals_count": 2,
      "goals_completed": 0,
      "goals_on_track": 2
    },
    {
      "name": "Clientes",
      "weight": 20,
      "score": 90,
      "goals_count": 2,
      "goals_completed": 1,
      "goals_on_track": 1
    },
    {
      "name": "Financeira",
      "weight": 10,
      "score": 80,
      "goals_count": 1,
      "goals_completed": 0,
      "goals_on_track": 1
    }
  ],
  "health_status": "green"
}
```

---

## Engagement (Check-ins e Feedback)

### 23. Listar Check-ins

**GET /engagement/checkins**

```http
GET /api/v1/engagement/checkins?employee_id=emp-001&from=2026-04-01&to=2026-04-13
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
X-Tenant-Id: tenant-001
```

**Response 200 OK**

```json
{
  "data": [
    {
      "id": "checkin-001",
      "employee_id": "emp-001",
      "employee_name": "Pedro Costa",
      "date": "2026-04-10T09:00:00Z",
      "mood": "positive",
      "energy_level": 8,
      "productivity_score": 9,
      "notes": "Semana produtiva. Finalizei refatoração do módulo de pagamentos.",
      "blockers": null,
      "support_needed": false
    }
  ],
  "meta": {
    "total": 8,
    "avg_mood": "positive",
    "avg_energy": 7.5,
    "avg_productivity": 8.2
  }
}
```

### 24. Criar Check-in

**POST /engagement/checkins**

```json
// Request
{
  "employee_id": "emp-001",
  "mood": "neutral",
  "energy_level": 6,
  "productivity_score": 7,
  "notes": "Semana com muitas reuniões. Produtividade afetada.",
  "blockers": "Muitas reuniões não-produtivas",
  "support_needed": true
}

// Response 201 Created
{
  "id": "checkin-150",
  "employee_id": "emp-001",
  "date": "2026-04-13T11:30:00Z",
  "mood": "neutral",
  "created_at": "2026-04-13T11:30:00Z"
}
```

### 25. Obter Pulso de Clima

**GET /engagement/pulse**

```http
GET /api/v1/engagement/pulse?department=Tecnologia&period=last_30_days
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
X-Tenant-Id: tenant-001
```

**Response 200 OK**

```json
{
  "period": "last_30_days",
  "department": "Tecnologia",
  "overall_health": "yellow",
  "metrics": {
    "avg_mood": "neutral",
    "avg_energy": 6.5,
    "avg_productivity": 7.2,
    "enps": 35,
    "participation_rate": 85
  },
  "trends": {
    "mood": "declining",
    "energy": "stable",
    "productivity": "improving"
  },
  "alerts": [
    {
      "type": "low_energy",
      "severity": "medium",
      "message": "Energia média caiu 15% nas últimas 2 semanas",
      "affected_employees": 12
    }
  ],
  "top_blockers": [
    "Muitas reuniões",
    "Falta de clareza nas prioridades",
    "Sobrecarga de trabalho"
  ]
}
```

### 26. Criar Feedback (1:1)

**POST /engagement/feedback**

```json
// Request
{
  "employee_id": "emp-001",
  "feedback_type": "one_on_one",
  "content": "Pedro demonstrou excelente progresso no PDI. Recomendo aumentar autonomia em decisões de arquitetura.",
  "strengths": [
    "Tomada de decisão",
    "Iniciativa",
    "Comunicação técnica"
  ],
  "development_areas": [
    "Delegação",
    "Paciência com processos longos"
  ],
  "next_steps": "Liderar discussão de arquitetura na próxima sprint",
  "visibility": "private"
}

// Response 201 Created
{
  "id": "feedback-001",
  "employee_id": "emp-001",
  "feedback_type": "one_on_one",
  "created_by": "emp-050",
  "created_at": "2026-04-13T11:35:00Z"
}
```

### 27. Listar Feedbacks de um Colaborador

**GET /engagement/feedback**

```http
GET /api/v1/engagement/feedback?employee_id=emp-001&type=one_on_one
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
X-Tenant-Id: tenant-001
```

**Response 200 OK**

```json
{
  "data": [
    {
      "id": "feedback-001",
      "employee_id": "emp-001",
      "employee_name": "Pedro Costa",
      "feedback_type": "one_on_one",
      "content": "Pedro demonstrou excelente progresso no PDI...",
      "created_by": {
        "id": "emp-050",
        "name": "Maria Santos"
      },
      "created_at": "2026-04-13T11:35:00Z"
    }
  ],
  "meta": {
    "total": 12
  }
}
```

---

## AI Copilot

### 28. Gerar Pauta de 1:1

**POST /ai/copilot/one-on-one-agenda**

```json
// Request
{
  "employee_id": "emp-001",
  "manager_id": "emp-050",
  "context": "quarterly_review"
}

// Response 200 OK
{
  "employee": {
    "id": "emp-001",
    "name": "Pedro Costa"
  },
  "manager": {
    "id": "emp-050",
    "name": "Maria Santos"
  },
  "generated_at": "2026-04-13T11:40:00Z",
  "agenda": {
    "duration_minutes": 45,
    "sections": [
      {
        "title": "Check-in Emocional",
        "duration_minutes": 5,
        "suggested_questions": [
          "Como você está se sentindo esta semana?",
          "Algo está te preocupando no trabalho ou fora dele?"
        ]
      },
      {
        "title": "Progresso no PDI",
        "duration_minutes": 20,
        "context": "Pedro está em 65% de progresso. Goal 'Arquitetura de Microservices' está avançado.",
        "suggested_questions": [
          "Como está o progresso da refatoração do módulo de pagamentos?",
          "Você conseguiu resolver o bloqueio sobre service mesh?",
          "Precisa de suporte adicional da arquitetura?"
        ]
      },
      {
        "title": "Feedback e Reconhecimento",
        "duration_minutes": 10,
        "context": "Última apresentação em tech talk foi muito bem recebida.",
        "talking_points": [
          "Reconhecer excelente progresso em comunicação técnica",
          "Feedback sobre liderança em discussões de arquitetura"
        ]
      },
      {
        "title": "Próximos Passos",
        "duration_minutes": 10,
        "suggested_actions": [
          "Finalizar refatoração do módulo de pagamentos até 20/04",
          "Agendar apresentação de arquitetura para diretoria",
          "Iniciar mentoria de desenvolvedor júnior"
        ]
      }
    ],
    "additional_insights": [
      "Pedro tem perfil DISC D/I: Seja direto e objetivo. Foque em resultados e próximas ações.",
      "Energia em 6/10 na última semana: Pergunte sobre sobrecarga e prioridades.",
      "BSC Score 82: Reconheça excelente performance."
    ]
  },
  "confidence_score": 0.92
}
```

### 29. Gerar Recomendações de Ação

**POST /ai/copilot/recommendations**

```json
// Request
{
  "employee_id": "emp-001",
  "context": "pdi_progress"
}

// Response 200 OK
{
  "employee": {
    "id": "emp-001",
    "name": "Pedro Costa"
  },
  "recommendations": [
    {
      "type": "action",
      "priority": "high",
      "title": "Resolver bloqueio em service mesh",
      "description": "Pedro mencionou bloqueio sobre decisão de service mesh. Agendar reunião com arquitetura.",
      "suggested_action": "Agendar reunião técnica com time de arquitetura para decisão sobre service mesh",
      "deadline": "2026-04-20"
    },
    {
      "type": "recognition",
      "priority": "medium",
      "title": "Reconhecer progresso em comunicação",
      "description": "Pedro evoluiu significativamente em comunicação executiva (nível 2 → 4).",
      "suggested_action": "Dar feedback positivo sobre apresentação na diretoria e recomendar que ele compartilhe práticas com o time"
    }
  ],
  "generated_at": "2026-04-13T11:45:00Z",
  "confidence_score": 0.88
}
```

### 30. Gerar Resumo Executivo de Equipe

**POST /ai/copilot/team-summary**

```json
// Request
{
  "manager_id": "emp-050",
  "period": "last_30_days"
}

// Response 200 OK
{
  "manager": {
    "id": "emp-050",
    "name": "Maria Santos"
  },
  "period": "last_30_days",
  "team_size": 5,
  "summary": {
    "overall_health": "yellow",
    "key_insights": [
      "2 membros com energia baixa (6/10). Investigar sobrecarga.",
      "PDI de Pedro Costa avançando muito bem (65%). Considerar promoção.",
      "Julia Fernandes precisa de suporte em onboarding de novas tecnologias."
    ],
    "action_items": [
      {
        "priority": "urgent",
        "action": "1:1 com Julia para entender dificuldades em React",
        "deadline": "2026-04-15"
      },
      {
        "priority": "high",
        "action": "Revisar carga de trabalho de Carlos e Ana (energia baixa)",
        "deadline": "2026-04-20"
      }
    ],
    "team_strengths": [
      "Equipe com foco forte em resultados (perfis D dominantes)",
      "Alta produtividade média (8.2/10)",
      "Progresso consistente em PDIs (média 62%)"
    ],
    "team_risks": [
      "Baixa diversidade de perfis DISC pode gerar conflitos",
      "Energia caindo nas últimas 2 semanas"
    ]
  },
  "generated_at": "2026-04-13T11:50:00Z"
}
```

### 31. Sugerir Ações de PDI

**POST /ai/copilot/pdi-suggestions**

```json
// Request
{
  "employee_id": "emp-150",
  "competency_id": "comp-soft-005",
  "current_level": 2,
  "target_level": 4
}

// Response 200 OK
{
  "employee": {
    "id": "emp-150",
    "name": "Ana Silva"
  },
  "competency": {
    "id": "comp-soft-005",
    "name": "Comunicação Executiva"
  },
  "current_level": 2,
  "target_level": 4,
  "gap_analysis": "Ana comunica de forma clara, mas ainda não estrutura apresentações de forma executiva. Necessário desenvolver storytelling e adaptação ao público.",
  "suggested_actions": [
    {
      "type": "70_on_job",
      "description": "Apresentar resultados de campanha em reunião de diretoria (com coaching prévio)",
      "estimated_impact": "high",
      "estimated_duration": "2 weeks"
    },
    {
      "type": "20_mentoring",
      "description": "Acompanhar apresentações do CEO em 3 reuniões e fazer debriefing com mentor",
      "estimated_impact": "medium",
      "estimated_duration": "1 month"
    },
    {
      "type": "10_formal",
      "description": "Curso online 'Executive Presentations' (LinkedIn Learning)",
      "estimated_impact": "medium",
      "estimated_duration": "1 week"
    }
  ],
  "estimated_time_to_target": "3-4 months",
  "generated_at": "2026-04-13T11:55:00Z"
}
```

---

## Schemas e Models

### Employee

```json
{
  "id": "uuid",
  "name": "string",
  "email": "string",
  "role": "string",
  "department": "string",
  "manager_id": "uuid | null",
  "hire_date": "date",
  "status": "enum: active | inactive | on_leave",
  "disc_profile": "enum: D | I | S | C | null",
  "avatar_url": "string | null",
  "created_at": "datetime",
  "updated_at": "datetime"
}
```

### DISCAssessment

```json
{
  "id": "uuid",
  "employee_id": "uuid",
  "status": "enum: pending | in_progress | completed | expired",
  "primary_profile": "enum: D | I | S | C | null",
  "secondary_profile": "enum: D | I | S | C | null",
  "scores": {
    "D": "int (0-100)",
    "I": "int (0-100)",
    "S": "int (0-100)",
    "C": "int (0-100)"
  },
  "interpretation": "object | null",
  "completed_at": "datetime | null",
  "report_url": "string | null",
  "created_at": "datetime"
}
```

### PDI

```json
{
  "id": "uuid",
  "employee_id": "uuid",
  "cycle": "string",
  "status": "enum: draft | active | completed | archived",
  "progress": "int (0-100)",
  "goals": "array[PDIGoal]",
  "created_at": "datetime",
  "updated_at": "datetime"
}
```

### PDIGoal

```json
{
  "id": "uuid",
  "pdi_id": "uuid",
  "title": "string",
  "competency_id": "uuid",
  "current_level": "int (1-5)",
  "target_level": "int (1-5)",
  "status": "enum: not_started | in_progress | completed | blocked",
  "progress": "int (0-100)",
  "actions": "array[PDIAction]",
  "checkpoints": "array[PDICheckpoint]",
  "created_at": "datetime"
}
```

### BSCGoal

```json
{
  "id": "uuid",
  "title": "string",
  "description": "string",
  "owner_id": "uuid",
  "bsc_perspective": "enum: Financeira | Clientes | Processos Internos | Aprendizado e Crescimento",
  "metric": {
    "name": "string",
    "unit": "string",
    "baseline": "float",
    "target": "float",
    "current": "float"
  },
  "progress": "int (0-100)",
  "status": "enum: not_started | on_track | at_risk | off_track | completed",
  "period": "string",
  "due_date": "date",
  "created_at": "datetime"
}
```

### CheckIn

```json
{
  "id": "uuid",
  "employee_id": "uuid",
  "date": "datetime",
  "mood": "enum: very_negative | negative | neutral | positive | very_positive",
  "energy_level": "int (1-10)",
  "productivity_score": "int (1-10)",
  "notes": "string | null",
  "blockers": "string | null",
  "support_needed": "boolean",
  "created_at": "datetime"
}
```

### Feedback

```json
{
  "id": "uuid",
  "employee_id": "uuid",
  "feedback_type": "enum: one_on_one | peer | self | 360",
  "content": "string",
  "strengths": "array[string] | null",
  "development_areas": "array[string] | null",
  "next_steps": "string | null",
  "visibility": "enum: private | shared_with_employee",
  "created_by": "uuid",
  "created_at": "datetime"
}
```

### Competency

```json
{
  "id": "uuid",
  "name": "string",
  "category": "enum: Técnica | Comportamental | Liderança",
  "description": "string",
  "levels": [
    {
      "level": "int (1-5)",
      "description": "string"
    }
  ],
  "is_custom": "boolean",
  "created_at": "datetime"
}
```

---

## Webhooks

### Eventos Disponíveis

O MCP-RH pode enviar webhooks para eventos importantes do sistema.

#### Configuração de Webhook

**POST /webhooks**

```json
// Request
{
  "url": "https://empresa.com/webhooks/mcp-rh",
  "events": [
    "pdi.created",
    "pdi.goal_completed",
    "checkin.low_energy_detected",
    "feedback.created"
  ],
  "secret": "webhook_secret_key_123"
}

// Response 201 Created
{
  "id": "webhook-001",
  "url": "https://empresa.com/webhooks/mcp-rh",
  "events": ["pdi.created", "pdi.goal_completed", "checkin.low_energy_detected", "feedback.created"],
  "status": "active",
  "created_at": "2026-04-13T12:00:00Z"
}
```

### Eventos e Payloads

#### 1. pdi.created

Disparado quando um novo PDI é criado.

```json
{
  "event": "pdi.created",
  "timestamp": "2026-04-13T12:05:00Z",
  "tenant_id": "tenant-001",
  "data": {
    "pdi_id": "pdi-150",
    "employee_id": "emp-001",
    "employee_name": "Pedro Costa",
    "cycle": "Q2-2026",
    "goals_count": 4
  }
}
```

#### 2. pdi.goal_completed

Disparado quando uma meta de PDI é concluída.

```json
{
  "event": "pdi.goal_completed",
  "timestamp": "2026-04-13T12:10:00Z",
  "tenant_id": "tenant-001",
  "data": {
    "pdi_id": "pdi-001",
    "goal_id": "goal-001",
    "employee_id": "emp-001",
    "employee_name": "Pedro Costa",
    "goal_title": "Dominar arquitetura de microservices",
    "completed_at": "2026-04-13T12:10:00Z"
  }
}
```

#### 3. checkin.low_energy_detected

Disparado quando um check-in indica energia baixa (≤4).

```json
{
  "event": "checkin.low_energy_detected",
  "timestamp": "2026-04-13T12:15:00Z",
  "tenant_id": "tenant-001",
  "data": {
    "checkin_id": "checkin-150",
    "employee_id": "emp-002",
    "employee_name": "Julia Fernandes",
    "energy_level": 3,
    "mood": "negative",
    "blockers": "Sobrecarga de trabalho",
    "support_needed": true
  }
}
```

#### 4. feedback.created

Disparado quando um novo feedback é criado.

```json
{
  "event": "feedback.created",
  "timestamp": "2026-04-13T12:20:00Z",
  "tenant_id": "tenant-001",
  "data": {
    "feedback_id": "feedback-150",
    "employee_id": "emp-001",
    "employee_name": "Pedro Costa",
    "feedback_type": "one_on_one",
    "created_by": "emp-050",
    "created_by_name": "Maria Santos"
  }
}
```

### Validação de Webhooks

Todos os webhooks incluem header `X-MCP-Signature` para validação:

```http
X-MCP-Signature: sha256=5d41402abc4b2a76b9719d911017c592
```

Validação:

```javascript
const crypto = require('crypto');

function validateWebhook(payload, signature, secret) {
  const hash = crypto
    .createHmac('sha256', secret)
    .update(JSON.stringify(payload))
    .digest('hex');

  return `sha256=${hash}` === signature;
}
```

---

## Códigos de Erro

### Estrutura de Erro

```json
{
  "error": {
    "code": "ERROR_CODE",
    "message": "Mensagem de erro legível",
    "details": "Detalhes adicionais (opcional)",
    "field": "campo_com_erro (opcional)",
    "timestamp": "2026-04-13T12:30:00Z"
  }
}
```

### Códigos HTTP e Erros

| Status | Code | Descrição |
|--------|------|-----------|
| **400** | `INVALID_REQUEST` | Request inválida (JSON malformado, campos faltando) |
| **400** | `VALIDATION_ERROR` | Validação de dados falhou |
| **401** | `UNAUTHORIZED` | Token ausente ou inválido |
| **401** | `TOKEN_EXPIRED` | Token JWT expirado |
| **401** | `INVALID_CREDENTIALS` | Credenciais inválidas (login) |
| **403** | `FORBIDDEN` | Usuário autenticado mas sem permissão |
| **403** | `TENANT_MISMATCH` | Tentativa de acesso a dados de outro tenant |
| **404** | `NOT_FOUND` | Recurso não encontrado |
| **409** | `CONFLICT` | Conflito (ex: e-mail já cadastrado) |
| **422** | `UNPROCESSABLE_ENTITY` | Lógica de negócio impede a operação |
| **429** | `RATE_LIMIT_EXCEEDED` | Rate limit excedido |
| **500** | `INTERNAL_SERVER_ERROR` | Erro interno do servidor |
| **503** | `SERVICE_UNAVAILABLE` | Serviço temporariamente indisponível |

### Exemplos de Erros

**400 Bad Request - Validação**

```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Falha na validação dos dados",
    "details": [
      {
        "field": "email",
        "message": "E-mail inválido"
      },
      {
        "field": "hire_date",
        "message": "Data de admissão não pode ser no futuro"
      }
    ],
    "timestamp": "2026-04-13T12:35:00Z"
  }
}
```

**403 Forbidden - Permissão**

```json
{
  "error": {
    "code": "FORBIDDEN",
    "message": "Você não tem permissão para acessar PDIs de outros colaboradores",
    "required_permission": "pdi:read_all",
    "timestamp": "2026-04-13T12:40:00Z"
  }
}
```

**422 Unprocessable Entity - Lógica de Negócio**

```json
{
  "error": {
    "code": "UNPROCESSABLE_ENTITY",
    "message": "Não é possível definir meta de nível inferior ao nível atual",
    "details": "Nível atual: 3, Nível alvo tentado: 2. Nível alvo deve ser maior que o atual.",
    "timestamp": "2026-04-13T12:45:00Z"
  }
}
```

---

## Especificação OpenAPI 3.0 Completa

```yaml
openapi: 3.0.3
info:
  title: MCP-RH API
  description: |
    API REST para o sistema MCP-RH (Multi-tenant) de gestão de capital humano.

    Inclui módulos de:
    - Identity & Tenant
    - People (Colaboradores e Lideranças)
    - DISC (Assessments Comportamentais)
    - PDI (Planos de Desenvolvimento Individual)
    - Performance (BSC e Metas)
    - Engagement (Check-ins, Feedback, Clima)
    - AI Copilot (Recomendações Assistidas por IA)
  version: 1.0.0
  contact:
    name: Suporte MCP-RH
    email: suporte@mcp-rh.com
    url: https://mcp-rh.com/docs
  license:
    name: Proprietário

servers:
  - url: https://api.mcp-rh.com/api/v1
    description: Produção
  - url: https://staging-api.mcp-rh.com/api/v1
    description: Staging
  - url: http://localhost:5000/api/v1
    description: Desenvolvimento Local

security:
  - BearerAuth: []
  - TenantId: []

paths:
  /auth/login:
    post:
      summary: Autenticar usuário
      tags: [Authentication]
      security: []
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              required: [email, password, tenant_slug]
              properties:
                email:
                  type: string
                  format: email
                password:
                  type: string
                  format: password
                tenant_slug:
                  type: string
      responses:
        '200':
          description: Autenticação bem-sucedida
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/AuthResponse'
        '401':
          $ref: '#/components/responses/Unauthorized'

  /employees:
    get:
      summary: Listar colaboradores
      tags: [People]
      parameters:
        - name: department
          in: query
          schema:
            type: string
        - name: status
          in: query
          schema:
            type: string
            enum: [active, inactive, on_leave]
        - name: page
          in: query
          schema:
            type: integer
            default: 1
        - name: limit
          in: query
          schema:
            type: integer
            default: 20
            maximum: 100
      responses:
        '200':
          description: Lista de colaboradores
          content:
            application/json:
              schema:
                type: object
                properties:
                  data:
                    type: array
                    items:
                      $ref: '#/components/schemas/Employee'
                  meta:
                    $ref: '#/components/schemas/PaginationMeta'
                  links:
                    $ref: '#/components/schemas/PaginationLinks'

  /pdi:
    get:
      summary: Listar PDIs
      tags: [PDI]
      parameters:
        - name: employee_id
          in: query
          schema:
            type: string
            format: uuid
        - name: status
          in: query
          schema:
            type: string
            enum: [draft, active, completed, archived]
      responses:
        '200':
          description: Lista de PDIs
          content:
            application/json:
              schema:
                type: object
                properties:
                  data:
                    type: array
                    items:
                      $ref: '#/components/schemas/PDI'

components:
  securitySchemes:
    BearerAuth:
      type: http
      scheme: bearer
      bearerFormat: JWT
    TenantId:
      type: apiKey
      in: header
      name: X-Tenant-Id

  schemas:
    AuthResponse:
      type: object
      properties:
        access_token:
          type: string
        refresh_token:
          type: string
        token_type:
          type: string
          example: Bearer
        expires_in:
          type: integer
        user:
          $ref: '#/components/schemas/User'

    User:
      type: object
      properties:
        id:
          type: string
          format: uuid
        name:
          type: string
        email:
          type: string
          format: email
        roles:
          type: array
          items:
            type: string

    Employee:
      type: object
      properties:
        id:
          type: string
          format: uuid
        name:
          type: string
        email:
          type: string
          format: email
        role:
          type: string
        department:
          type: string
        status:
          type: string
          enum: [active, inactive, on_leave]
        disc_profile:
          type: string
          enum: [D, I, S, C]
          nullable: true

    PDI:
      type: object
      properties:
        id:
          type: string
          format: uuid
        employee_id:
          type: string
          format: uuid
        cycle:
          type: string
        status:
          type: string
          enum: [draft, active, completed, archived]
        progress:
          type: integer
          minimum: 0
          maximum: 100

    PaginationMeta:
      type: object
      properties:
        current_page:
          type: integer
        per_page:
          type: integer
        total:
          type: integer
        total_pages:
          type: integer

    PaginationLinks:
      type: object
      properties:
        first:
          type: string
          format: uri
        last:
          type: string
          format: uri
        prev:
          type: string
          format: uri
          nullable: true
        next:
          type: string
          format: uri
          nullable: true

    Error:
      type: object
      properties:
        error:
          type: object
          properties:
            code:
              type: string
            message:
              type: string
            timestamp:
              type: string
              format: date-time

  responses:
    Unauthorized:
      description: Não autenticado
      content:
        application/json:
          schema:
            $ref: '#/components/schemas/Error'

    Forbidden:
      description: Sem permissão
      content:
        application/json:
          schema:
            $ref: '#/components/schemas/Error'
```

---

## Guia de Uso

### Exemplo de Fluxo Completo

#### 1. Autenticação

```bash
curl -X POST https://api.mcp-rh.com/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "usuario@empresa.com",
    "password": "senha123",
    "tenant_slug": "empresa-abc"
  }'
```

#### 2. Listar Colaboradores

```bash
curl -X GET "https://api.mcp-rh.com/api/v1/employees?department=Tech&status=active" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..." \
  -H "X-Tenant-Id: tenant-001"
```

#### 3. Criar PDI

```bash
curl -X POST https://api.mcp-rh.com/api/v1/pdi \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..." \
  -H "X-Tenant-Id: tenant-001" \
  -H "Content-Type: application/json" \
  -d '{
    "employee_id": "emp-001",
    "cycle": "Q2-2026",
    "goals": [...]
  }'
```

#### 4. Gerar Pauta de 1:1 com IA

```bash
curl -X POST https://api.mcp-rh.com/api/v1/ai/copilot/one-on-one-agenda \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..." \
  -H "X-Tenant-Id: tenant-001" \
  -H "Content-Type: application/json" \
  -d '{
    "employee_id": "emp-001",
    "manager_id": "emp-050",
    "context": "quarterly_review"
  }'
```

---

## Considerações Finais

### Próximos Passos

1. **Implementação da API REST** em ASP.NET Core conforme especificação
2. **Testes de integração** com Postman/Insomnia
3. **Documentação interativa** com Swagger UI
4. **SDKs cliente** (JavaScript, Python, C#)

### Changelog

| Versão | Data | Alterações |
|--------|------|-----------|
| 1.0.0 | 2026-04-13 | Especificação inicial completa |

---

**Documento gerado em:** 2026-04-13
**Baseado em:** Documentação MCP-RH (`mcp-rh/docs/`)
**Autor:** Claude Sonnet 4.5
**Formato:** OpenAPI 3.0 + Markdown
