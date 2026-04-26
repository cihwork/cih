# Análise Semântica Profunda - MCP-RH

**Data:** 2026-04-13
**Fonte:** Documentação em `/mcp-rh/docs/` (7 documentos markdown)
**Tipo:** Análise de proposta de valor, arquitetura de produto e visão estratégica

---

## 📋 Síntese Executiva

O MCP-RH não é um sistema de RH tradicional. É um **orquestrador de desenvolvimento humano** que transforma práticas de consultoria em sistema operacional de liderança.

### Tese Central (do documento)

> "O produto não deve competir com um HRIS tradicional. Ele deve funcionar como um orquestrador de desenvolvimento humano."

### O Que Isso Significa na Prática

Não é folha de pagamento, ponto eletrônico ou admissão/demissão.
**É:** PDI vivo + Feedback estruturado + Liderança assistida por IA + Calibração cultural.

---

## 1️⃣ Problema Raiz (Análise Semântica)

### O Que a Documentação Revela

**Fonte:** `01-fonte-e-problema.md`

> "Hoje o mercado ainda trata desenvolvimento como documento."

#### Sintomas Identificados

| Sintoma | O Que Isso Significa | Impacto Real |
|---------|---------------------|--------------|
| **PDI feito para cumprir tabela** | Ritual anual sem consequência | Desenvolvimento fake |
| **Feedback sem frequência** | Conversa de corredor ou anual | Sem calibração de rota |
| **Liderança sem leitura humana** | Gestor decide por feeling | Inconsistência e viés |
| **Metas desconectadas do crescimento** | OKR não conversa com PDI | Dualidade tática vs. desenvolvimento |
| **Falta de visão integrada** | Silos entre comportamento, competência e resultado | RH reativo, não estratégico |

#### Insight Semântico

O problema não é **falta de processos**. É **falta de operacionalização**.

As empresas **sabem** fazer PDI, DISC, BSC. Mas fazem em:
- Excel espalhado
- Templates estáticos
- Reuniões sem histórico
- Dashboards sem ação

**O gap é infraestrutura de execução**, não de metodologia.

---

## 2️⃣ Proposta de Valor (Arquitetura Semântica)

### O Que o Produto Promete

**Fonte:** `02-visao-do-produto.md`

#### Para Cada Persona

| Persona | Dor Atual | Promessa do MCP-RH | Resultado Esperado |
|---------|-----------|-------------------|-------------------|
| **BP/RH Estratégico** | "Não enxergo risco humano antes da crise" | Visão consolidada de carteira + Semáforo de saúde | Antecipação, não reação |
| **Coach/Consultor** | "Cada empresa é uma metodologia diferente" | Metodologia padronizada + Histórico + Evidências | Consistência e rastreabilidade |
| **Líder** | "Não sei o que falar no 1:1" | Pauta pronta + Recomendações + Semáforo de equipe | 1:1 com propósito, não improviso |
| **Colaborador** | "Não sei se estou evoluindo" | Trilha clara + Acompanhamento justo + Evidências | Clareza de progressão |

#### Tradução Semântica

**Problema de fundo:** Sobrecarga cognitiva de líderes e RH.
**Solução:** Redução de fricção operacional sem perder o humano.

> "Padronizar a leitura humana sem desumanizar a relação."

Essa é a tensão central do produto: **Escala + Empatia**.

---

## 3️⃣ Diferenciação Competitiva (O Que Não É Óbvio)

### Posicionamento Estratégico

**Fonte:** `00-resumo-executivo.md`, `02-visao-do-produto.md`

#### Não Compete Com

- ❌ HRIS (Workday, SAP SuccessFactors, TOTVS)
- ❌ Folha de pagamento (Gupy, Factorial)
- ❌ ATS (Recrutamento e seleção)

#### Compete/Complementa

- 🔄 Ferramentas de OKR (Perdoo, Gtmhub) — mas conecta com PDI
- 🔄 Plataformas de LMS (EdCast, Degreed) — mas contextualiza com competências
- 🔄 Ferramentas de feedback (Officevibe, 15Five) — mas integra com desenvolvimento

#### Diferencial Único (Análise Semântica)

**"IA como copiloto de BP, coach e líder"**

| O Que Outros Fazem | O Que o MCP-RH Propõe |
|-------------------|---------------------|
| Dashboard de dados | **Recomendações acionáveis** |
| Relatório de clima | **Alertas de risco humano com contexto** |
| Template de 1:1 | **Pauta gerada com histórico da pessoa** |
| DISC como relatório | **DISC como calibração de comunicação** |

**Insight:** A IA não substitui o líder. Ela reduz o custo cognitivo de preparação.

---

## 4️⃣ Arquitetura de Produto (Visão Semântica)

### 5 Movimentos Integrados

**Fonte:** `00-resumo-executivo.md`

```
┌─────────────────────────────────────────┐
│  1. PDI contínuo (não anual)            │  ← Ritmo
├─────────────────────────────────────────┤
│  2. DISC como ajuste (não rótulo)       │  ← Linguagem
├─────────────────────────────────────────┤
│  3. BSC/Metas = Conexão com estratégia  │  ← Alinhamento
├─────────────────────────────────────────┤
│  4. EC/Maturidade = Calibração cultural │  ← Contexto
├─────────────────────────────────────────┤
│  5. IA como copiloto                    │  ← Aceleração
└─────────────────────────────────────────┘
```

#### Análise Semântica dos Movimentos

| Movimento | O Que Significa | Por Que Importa |
|-----------|----------------|-----------------|
| **PDI contínuo** | Semanal/quinzenal, não anual | Desenvolvimento é processo, não evento |
| **DISC como linguagem** | Ajuste de comunicação, não etiqueta | "Você é D" vs. "Em X contexto, comportamento D funciona" |
| **BSC conectado** | Metas estratégicas informam PDI | Crescimento alinhado com negócio |
| **EC como calibração** | Maturidade da cultura define como aplicar | Startup != corporação tradicional |
| **IA como copiloto** | Assistência, não automação | Líder decide, IA sugere |

---

## 5️⃣ Módulos e Experiência (Arquitetura de Jornada)

### 10 Módulos Principais

**Fonte:** `03-modulos-e-experiencia.md`

| # | Módulo | Tipo | Finalidade Semântica |
|---|--------|------|---------------------|
| 1 | Tenant e organização | Infraestrutura | Isolamento multi-tenant |
| 2 | Identidade, acesso e papéis | Infraestrutura | Segurança e governança |
| 3 | Perfil comportamental DISC | Assessment | Leitura de comportamento |
| 4 | Consciência organizacional (EC) | Assessment | Calibração cultural |
| 5 | BSC, metas e indicadores | Performance | Alinhamento estratégico |
| 6 | PDI contínuo por competência | Desenvolvimento | Motor do produto |
| 7 | Check-ins, 1:1 e feedback | Engajamento | Ritmo de calibração |
| 8 | Pulso de clima, energia e risco | Monitoramento | Antecipação de crise |
| 9 | Recomendações assistidas por IA | Copiloto | Redução de fricção |
| 10 | Integrações MCP e canais | Conectividade | IA externa acessa dados |

#### Insight Semântico: A Arquitetura É de Camadas, Não de Silos

```
┌─────────────────────────────────────────┐
│     MCP Gateway (IA externa)            │  ← Exposição
├─────────────────────────────────────────┤
│     AI Copilot (Recomendações)          │  ← Inteligência
├─────────────────────────────────────────┤
│  Engagement (Clima, Pulso, Check-in)    │  ← Ritmo
├─────────────────────────────────────────┤
│  Performance (BSC, Metas)               │  ← Resultado
├─────────────────────────────────────────┤
│  PDI (Competências, Gaps, Checkpoints)  │  ← Motor
├─────────────────────────────────────────┤
│  Assessments (DISC, EC)                 │  ← Contexto
├─────────────────────────────────────────┤
│  Core People (Pessoas, Lideranças)      │  ← Base
├─────────────────────────────────────────┤
│  Identity & Tenant                      │  ← Fundação
└─────────────────────────────────────────┘
```

**Cada camada depende da anterior, mas não se mistura.**

---

## 6️⃣ Linguagem do Produto (Análise Semântica de Tom)

### O Que o Produto Deve Falar

**Fonte:** `03-modulos-e-experiencia.md` (linha 44-56)

| ✅ Fala Em | ❌ Não Fala Em |
|-----------|---------------|
| Competência | Tarefa |
| Comportamento | Ponto |
| Resultado | Nota |
| Maturidade | — |
| Evolução | — |

#### Análise: Por Que Isso Importa?

**Semântica de "Competência" vs. "Tarefa":**
- Tarefa: "Fazer relatório semanal"
- Competência: "Comunicação executiva clara e objetiva"

**Semântica de "Evolução" vs. "Nota":**
- Nota: Comparação com outros (ranking tóxico)
- Evolução: Comparação consigo mesmo (crescimento)

**Insight:** A linguagem define se o produto humaniza ou gamifica pessoas.

---

## 7️⃣ Arquitetura Multi-Tenant (Análise de Decisões)

### Princípios Técnicos com Significado de Negócio

**Fonte:** `04-arquitetura-multi-tenant.md`

| Princípio Técnico | Significado de Negócio |
|------------------|----------------------|
| **Isolamento por tenant desde o início** | Cada cliente é um mundo. Dados nunca vazam. |
| **Domínio humano separado de operacional** | Desenvolvimento != Administração |
| **Auditoria em toda ação sensível** | Compliance (LGPD, trabalhista) |
| **Permissão por papel e contexto** | Líder vê equipe. BP vê carteira. Colaborador vê só seu PDI. |
| **IA como assistente, não decisor** | Transparência: "Por que você sugeriu isso?" |

#### Camadas Sugeridas (Tradução Semântica)

| Camada Técnica | O Que Ela Entrega de Valor |
|---------------|---------------------------|
| **Identity e tenant resolution** | "Quem é você e de qual empresa?" |
| **Core People** | "Quem lidera quem? Quem está em qual time?" |
| **Assessments (DISC, EC)** | "Como essa pessoa se comporta? Como a cultura funciona?" |
| **PDI** | "O que essa pessoa precisa desenvolver? Como acompanhar?" |
| **Performance (BSC)** | "As metas estratégicas estão avançando?" |
| **Engagement** | "A pessoa está ok? O time está ok?" |
| **AI Copilot** | "O que o líder/BP/coach deve fazer agora?" |
| **MCP Gateway** | "Como outras IAs podem usar esses dados?" |

---

## 8️⃣ Governança e Ética (Análise de Riscos)

### Guardrails Explícitos

**Fonte:** `06-governanca.md`

#### Riscos Ativamente Evitados

| Risco | Como Evitar | Por Que Isso É Crítico |
|-------|------------|----------------------|
| **DISC vira rótulo fixo** | "Você é D" → "Em situação X, comportamento D aparece" | Evita desumanização |
| **Clima vira arma** | Opt-in explícito + Segregação de dados | Evita uso punitivo |
| **PDI vira ferramenta disciplinar** | Separação entre desenvolvimento e advertência | PDI ≠ PIP (Performance Improvement Plan) |
| **Ranking tóxico de pessoas** | Sem "top 10%" ou "bottom 10%" | Evita competição destrutiva |

#### Análise Semântica: O Que Isso Revela?

Esses guardrails **não são features**. São **limites arquiteturais**.

**Exemplo:**
```csharp
// NÃO deve existir no código
public List<Employee> GetBottomPerformers(int percentage)
{
    // ❌ Isso viola o princípio de "sem ranking tóxico"
}

// DEVE existir
public PersonalDevelopmentInsights GetIndividualProgress(EmployeeId id)
{
    // ✅ Foco em evolução individual, não comparação
}
```

**Insight:** A ética está no design do sistema, não só nas políticas.

---

## 9️⃣ Roadmap Estratégico (Análise de Prioridades)

### 5 Fases Sequenciais

**Fonte:** `05-roadmap.md`

| Fase | Entregáveis | Significado Estratégico |
|------|------------|------------------------|
| **1. Fundação** | Tenant, login, pessoas, competências | Sem isso, nada funciona |
| **2. PDI vivo** | Ciclo, metas, checkpoints, evidências | **Motor do produto** |
| **3. Liderança assistida** | 1:1 guiado, feedback, resumo semanal | **Diferencial competitivo** |
| **4. DISC, EC e calibração** | Perfil, maturidade, alertas | **Inteligência contextual** |
| **5. MCP e IA** | Ferramentas, prompts, resumo executivo | **Orquestração externa** |

#### Análise: O Que Entra Primeiro?

**Ordem declarada:**
1. Tenant
2. Pessoas
3. PDI
4. Check-ins
5. Dashboard executivo

**Tradução:** MVP é **PDI operacional + Ritmo de acompanhamento**.

Sem PDI vivo, não há produto. Sem check-ins, o PDI vira documento estático.

---

## 🔍 Análise de Gaps e Oportunidades

### O Que Está Implícito (Mas Não Explícito)

| Tema | O Que Falta na Documentação | Impacto |
|------|---------------------------|---------|
| **Integrações com HRIS** | Como conectar com folha de pagamento? | Dados de cargo/salário influenciam PDI |
| **Modelo de precificação** | SaaS por usuário? Por tenant? | Viabilidade econômica |
| **Onboarding de tenant** | Como configurar competências? | Fricção de adoção |
| **Migração de dados** | Como importar PDIs existentes? | Entrada de clientes legados |
| **Mobile first?** | Check-in rápido no celular? | UX de adoção |
| **Multilíngue?** | Empresas multinacionais | Escalabilidade geográfica |

### Oportunidades Semânticas (Expansão Futura)

1. **Planos de Sucessão**
   - DISC + PDI + Performance = Pronto para promoção?

2. **Mapeamento de Gaps de Equipe**
   - "Meu time tem 5 pessoas com perfil D, nenhuma S. Risco de fricção?"

3. **Biblioteca de Competências por Setor**
   - Tech: "Arquitetura de software", "Comunicação técnica"
   - Vendas: "Negociação", "Resiliência"

4. **IA para Sugestão de Plano 70-20-10**
   - "Para desenvolver X, sugiro: 70% projeto Y, 20% mentoria com Z, 10% curso W"

---

## 🎯 KPIs do Produto (Análise de Sucesso)

### Métricas Declaradas

**Fonte:** `06-governanca.md`

| KPI | O Que Mede | Por Que Importa |
|-----|-----------|-----------------|
| **Aderência ao PDI** | % de PDIs com progresso registrado | Produto está sendo usado? |
| **Taxa de check-in concluído** | % de check-ins realizados vs. agendados | Ritmo está funcionando? |
| **Progresso por competência** | Média de evolução em competências-chave | Desenvolvimento está acontecendo? |
| **Completude de 1:1** | % de 1:1s com pauta, ações e follow-up | Liderança está engajada? |
| **Score de engajamento do líder** | Uso de recomendações e ações tomadas | IA está sendo útil? |
| **Nível de risco humano por time** | Semáforo vermelho/amarelo/verde | Antecipação está funcionando? |
| **Evolução do semáforo executivo** | Melhoria no indicador ao longo do tempo | Produto gera impacto? |

### Critério de Sucesso (Análise Semântica)

> "O sistema é bem-sucedido quando a empresa consegue fazer a pessoa evoluir com mais clareza, menos improviso e melhor leitura de contexto."

**Tradução:** Sucesso = Clareza + Cadência + Contextualização

---

## 💡 Insights Estratégicos (Síntese Final)

### 1. **O Produto É Uma Tese Sobre Liderança**

Não é software de RH. É uma aposta de que:
- Líderes querem fazer bem, mas não têm tempo/método
- IA pode reduzir fricção sem desumanizar
- Desenvolvimento precisa de infraestrutura, não só vontade

### 2. **A Complexidade Está na Simplicidade**

O desafio não é construir funcionalidades. É fazer com que:
- 1:1 não vire burocracia
- DISC não vire rótulo
- Clima não vire arma

**Isso é design de produto, não engenharia de software.**

### 3. **O Moat (Diferencial Defensável) É Contextualização**

Qualquer ferramenta pode fazer PDI, DISC, BSC.
Poucas podem **calibrar por maturidade cultural (EC)**.

**Exemplo:**
- Startup early-stage: PDI ultra-flexível, foco em velocidade
- Empresa madura: PDI estruturado, foco em governança

**Se o produto acertar isso, cria lock-in de valor.**

### 4. **A IA Não É Feature, É Postura**

> "IA como assistente, não como decisor absoluto"

Isso não é disclaimer legal. É decisão de arquitetura.

**Implicação técnica:**
```
❌ Sistema automaticamente promove/rebaixa
✅ Sistema sugere, líder decide, sistema registra decisão
```

### 5. **O Maior Risco É Ser Genérico**

Se o MCP-RH tentar ser "tudo para todos":
- ❌ Vira HRIS fraco
- ❌ Vira LMS fraco
- ❌ Vira ferramenta de feedback fraca

Se focar em **liderança assistida + PDI vivo**:
- ✅ Cria categoria nova
- ✅ Complementa HRIS existente
- ✅ Tem posicionamento claro

---

## 📊 Matriz de Aderência (Proposta vs. Realidade)

### O Que a Documentação Promete vs. O Que Precisa Ser Validado

| Promessa | Status | Próximo Passo |
|----------|--------|---------------|
| **PDI contínuo, não anual** | 📘 Conceito | Validar com cliente: Ritmo semanal é viável? |
| **DISC como ajuste, não rótulo** | 📘 Conceito | Prototipar UX: Como apresentar DISC sem rotular? |
| **IA como copiloto** | 📘 Conceito | POC: Azure OpenAI gera pauta de 1:1 útil? |
| **Multi-tenant robusto** | 📘 Conceito | Validar: Finbuckle.MultiTenant resolve? |
| **Calibração por EC** | 📘 Conceito | Pesquisa: Como medir "nível de consciência"? |
| **MCP Gateway** | 📘 Conceito | Definir: Quais ferramentas de IA vão consumir? |

**Status Legend:**
- 📘 Conceito: Bem documentado, mas não validado
- 🚧 Em construção: Código inicial existe
- ✅ Validado: Cliente/usuário confirmou valor

---

## 🚧 Gaps Críticos (O Que Falta para Executar)

### 1. **Modelo de Dados Não Definido**

**Exemplo:** Como representar "Competência" no banco?

```sql
-- Competência genérica?
CREATE TABLE competences (
    id UUID,
    name VARCHAR(100),
    category VARCHAR(50)  -- Técnica, Comportamental, Liderança
);

-- Competência por tenant?
CREATE TABLE tenant_competences (
    id UUID,
    tenant_id UUID,
    name VARCHAR(100),
    description TEXT,
    rubric JSONB  -- Como avaliar de 1 a 5?
);
```

**Decisão necessária:** Biblioteca global + Customização por tenant?

### 2. **Modelo de Precificação Não Definido**

**Opções:**
- Por usuário ativo mensal (PAM)
- Por tenant (empresa) + tier por features
- Freemium + Enterprise

**Impacta arquitetura:** Controle de features por tenant.

### 3. **Integração com HRIS Não Especificada**

**Como sincronizar:**
- Estrutura organizacional (quem lidera quem)
- Mudanças de cargo/promoções
- Admissões/desligamentos

**Opções:**
- API REST bidirecional
- Webhooks
- Integração via Zapier/n8n

### 4. **Rubrica de Avaliação de Competências Não Definida**

**Exemplo:** "Comunicação executiva"

| Nível | Descrição |
|-------|-----------|
| 1 | Comunica informações básicas, mas sem clareza |
| 2 | Comunica de forma clara, mas sem estrutura |
| 3 | Comunica de forma estruturada e objetiva |
| 4 | Comunica com impacto, adaptando ao público |
| 5 | Referência de comunicação, influencia decisões |

**Decisão:** Rubrica fixa ou customizável por tenant?

---

## ✅ Pontos Fortes da Documentação

### 1. **Clareza de Propósito**

Não há ambiguidade sobre o que o produto é/não é.

### 2. **Governança como Princípio, Não Afterthought**

Ética e riscos discutidos antes de features.

### 3. **Foco em Jornada, Não em Features**

Documentação descreve **o que cada persona consegue fazer**, não lista de funcionalidades.

### 4. **Roadmap Incremental Realista**

Fases sequenciais, não tentativa de fazer tudo de uma vez.

### 5. **Linguagem Consistente**

"Competência, comportamento, resultado, maturidade, evolução" — não varia.

---

## ⚠️ Riscos Semânticos (O Que Pode Dar Errado)

### 1. **"PDI Vivo" Virar Microgerenciamento**

**Risco:** Check-in semanal virar controle tóxico.

**Mitigação:** UX precisa ser opt-in, não vigilância.

### 2. **DISC Virar Rótulo Mesmo Com Guardrails**

**Risco:** Líder falar "Ah, fulano é D, então é agressivo".

**Mitigação:** Sistema nunca mostra DISC sem contexto + explicação.

### 3. **IA Gerar Recomendação Enviesada**

**Risco:** Copilot sugere "Mulher X precisa ser mais assertiva" (viés de gênero).

**Mitigação:** Auditoria de prompts + Explicabilidade obrigatória.

### 4. **Adoção Ser Barrada por Fricção de Setup**

**Risco:** Empresa testa, mas configurar competências dá muito trabalho.

**Mitigação:** Biblioteca pré-configurada por setor + Wizard de onboarding.

---

## 🎯 Recomendações (Para Próximos Passos)

### Imediato (Esta Semana)

1. ✅ **Definir modelo de dados de "Competência"**
   - Genérica + Customização por tenant
   - Rubrica de avaliação (1-5)

2. ✅ **Criar wireframes de jornada crítica**
   - Líder fazendo 1:1 com pauta gerada por IA
   - Colaborador registrando evidência de progresso

3. ✅ **Prototipar prompt de IA Copilot**
   - Input: PDI do colaborador + Histórico de check-ins
   - Output: Pauta de 1:1 com 3-4 tópicos

### Curto Prazo (1-2 Semanas)

1. ✅ **Validar tese com 3 clientes potenciais**
   - BP de RH: "Você usaria isso?"
   - Líder: "Pauta gerada por IA ajuda?"
   - Colaborador: "Você acompanharia seu PDI por app?"

2. ✅ **Definir modelo de precificação**
   - SaaS B2B: Por tenant + tier por features
   - Freemium: Até 10 usuários grátis?

3. ✅ **Especificar integração com HRIS**
   - API REST: Estrutura org + Mudanças de cargo
   - Webhook: Admissões/desligamentos

### Médio Prazo (1 Mês)

1. ✅ **MVP funcional (Fase 1 + 2)**
   - Tenant + Pessoas + PDI básico + Check-in

2. ✅ **POC de IA Copilot**
   - Azure OpenAI: Geração de pauta de 1:1

3. ✅ **Biblioteca de competências**
   - Tech: 20 competências
   - Vendas: 15 competências
   - Liderança: 10 competências

---

## 📚 Fontes de Validação (O Que Falta)

### Pesquisas Citadas (Não Anexadas)

**Fonte:** `01-fonte-e-problema.md`

> "Este blueprint foi condensado a partir de:
> - pesquisa de PDI e gestão de desempenho;
> - estudo de DISC e calibração por nível de consciência;
> - guia de gestão de desempenho;
> - análise do gap do Cadence para desenvolvimento humano;
> - materiais de consultoria, contratos, relatórios e templates de acompanhamento."

**Recomendação:** Adicionar anexos com:
- Sumário da pesquisa de PDI
- Benchmarks de mercado (ferramentas concorrentes)
- Templates de consultoria usados como base

---

## 📊 Scorecard Final (Qualidade da Documentação)

| Dimensão | Nota | Justificativa |
|----------|------|---------------|
| **Clareza de Propósito** | 10/10 | Não há ambiguidade |
| **Tese de Negócio** | 9/10 | Forte, mas falta validação de mercado |
| **Arquitetura de Produto** | 10/10 | Camadas bem definidas |
| **Jornadas de Usuário** | 9/10 | Claras, mas faltam wireframes |
| **Governança e Ética** | 10/10 | Riscos ativamente mitigados |
| **Roadmap** | 8/10 | Realista, mas falta estimativa de esforço |
| **Viabilidade Técnica** | 7/10 | Falta modelo de dados e integrações |
| **Go-to-Market** | 5/10 | Falta modelo de precificação e validação de mercado |

**Média Geral:** **8.5/10**

**Classificação:** 📘 **Blueprint de Altíssima Qualidade** — Pronto para pitch executivo, mas precisa validação técnica e de mercado antes de desenvolvimento.

---

## 🏁 Conclusão Semântica

### O Que Este Documento Revela

O MCP-RH **não é um produto de software**.
É uma **tese sobre como liderança pode ser estruturada com tecnologia sem perder humanidade**.

### 3 Apostas Centrais

1. **PDI vivo é viável operacionalmente**
   - Ritmo semanal/quinzenal não vira fardo

2. **IA pode reduzir fricção de liderança**
   - Pauta de 1:1 gerada por IA é útil, não robótica

3. **Calibração cultural (EC) diferencia o produto**
   - Startups e corporações usam PDI diferente

### Se Essas Apostas Forem Verdadeiras

✅ O produto cria categoria nova
✅ Complementa (não compete com) HRIS
✅ Tem moat defensável (contextualização cultural)

### Se Essas Apostas Forem Falsas

❌ Vira ferramenta de PDI genérica
❌ Perde para concorrentes estabelecidos
❌ IA vira feature, não diferencial

---

**Próximo Passo Crítico:** **Validar a aposta de PDI vivo com cliente piloto.**

Sem isso, o produto é lindo no papel, mas não sabemos se funciona na prática.

---

**Documento gerado em:** 2026-04-13
**Baseado em:** 7 documentos markdown (`mcp-rh/docs/`)
**Análise por:** Claude Sonnet 4.5
**Tipo:** Análise semântica de proposta de valor e arquitetura de produto
