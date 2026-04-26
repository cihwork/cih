# Matriz de Aderência - MCP RH

**Data**: 2026-04-07
**Objetivo**: Avaliar se o **MCP-RH** (blueprint planejado) comporta funcionalidades do Sistema Operacional de RH

## Legenda

- ✅ **Resolve completamente** - Funcionalidade já existe e atende
- 🟡 **Resolve parcialmente** - Existe mas precisa de ajustes/melhorias
- ⚠️ **Não resolve, mas é adaptável** - Não existe, mas a base comporta
- ❌ **Não resolve** - Requer nova implementação/arquitetura

---

## Funcionalidades Avaliadas

### 1. Calibrar perguntas do PDI com base no nível de maturidade organizacional

**Status**: ⚠️ **Não resolve, mas é adaptável**

**Análise**:
- ✅ **Camada Assessments** prevê "EC e leitura de contexto" (arquitetura multi-tenant)
- ✅ **Camada PDI** prevê "competências, gaps, ciclos, ações e checkpoints"
- ✅ **Posicionamento** menciona "calibração de liderança por maturidade"
- ❌ Não há especificação de COMO as perguntas serão calibradas por EC

**O que falta detalhar**:
- Entidades: `OrganizationalMaturityLevel`, `ECContextualQuestionBank`, `QuestionCalibration`
- Motor de seleção de perguntas baseado em nível EC da organização
- Mapeamento: `ECLevel` → `PDIQuestionSet`
- Lógica: perguntas mais complexas para EC mais evoluída, mais práticas para EC inicial

**Próximo passo**: Especificar na camada Assessments a engine de calibração contextual

---

### 2. Escolher ângulo de avaliação: Competência → Liderados vs Liderado → Competências

**Status**: ⚠️ **Não resolve, mas é adaptável**

**Análise**:
- ✅ **Camada Core People** prevê "pessoas, cargos, lideranças e relações"
- ✅ **Camada PDI** prevê "competências, gaps..."
- ✅ **Experiência BP** menciona "visão consolidada da carteira"
- ✅ **Experiência Líder** menciona "semáforo de equipe"
- ❌ Não especifica explicitamente as duas visões/ângulos de análise

**O que falta detalhar**:
- **Visão 1 (Competência-first)**: Para cada competência, matriz de todos os liderados com notas/status
- **Visão 2 (People-first)**: Para cada liderado, radar/perfil de todas as competências
- UI com toggle entre perspectivas
- Queries otimizadas para cada ângulo:
  - `GetCompetencyMatrix(competencyId)` → retorna todos os people
  - `GetPersonCompetencyProfile(personId)` → retorna todas as competências

**Próximo passo**: Especificar na camada PDI as duas interfaces de consulta/visualização

---

### 3. Sistema de perguntas compactadas com validação cruzada

**Status**: ⚠️ **Não resolve, mas é adaptável**

**Análise**:
- ✅ **Camada PDI** prevê competências e assessments
- ✅ **Camada AI Copilot** pode assistir na geração de perguntas
- ❌ Não há especificação de sistema de perguntas comparativas/ranking
- ❌ Não há engine de validação cruzada

**Requisitos identificados**:

#### A. Perguntas Comparativas (Compactação)
Em vez de avaliar individualmente (lento), usar perguntas que avaliam múltiplos de uma vez:

**Tipos de perguntas**:
1. **Top N**: "Dos colaboradores abaixo, selecione os 3 que MAIS [competência]"
2. **Bottom N**: "Selecione os 3 que MAIS PRECISAM DESENVOLVER [competência]"
3. **Distribuição**: "Distribua os colaboradores em: Forte / Médio / Precisa desenvolver"
4. **Destaque**: "Quem se destaca positivamente em [competência]?"
5. **Atenção**: "Quem precisa de suporte urgente em [competência]?"

**Benefícios**:
- ⚡ Rápido: 1 pergunta avalia N pessoas
- 🎯 Preciso: Força escolhas claras (ranking vs escala)
- 🧠 Inteligente: Captura comparação relativa no contexto da equipe

#### B. Contra Perguntas de Validação
Sistema de validação cruzada para detectar inconsistências e viés:

**Tipos de validação**:
1. **Inversão**: "Mais X" vs "Menos X" (detecta contradição)
2. **Correlação**: Competências relacionadas (ex: liderança ↔ iniciativa)
3. **Contexto**: Resultado vs comportamento (ex: entrega ↔ resiliência)
4. **Temporal**: Mudanças desde última avaliação
5. **Perspectiva**: Visão do líder vs auto-avaliação

**Engine de validação**:
- Score de confiabilidade da avaliação
- Detecção de viés (halo effect, recency bias)
- Alertas para revisão de inconsistências
- Calibração automática de respostas

#### C. Abordagem Recomendada: HÍBRIDO

**Camada 1 - Core (MCP pre-definido)**:
- Pares de validação obrigatórios e fundamentais
- Garantia de consistência, auditoria e compliance
- Zero custo de IA, performance máxima

**Camada 2 - Inteligente (IA complementar)**:
- Sugestões de validações contextuais adicionais
- Detecção de padrões não mapeados
- Ajuste fino por tenant/setor

**Camada 3 - Feedback Loop**:
1. MCP carrega pares obrigatórios
2. IA sugere validações extras (marcadas como "sugeridas")
3. BP/Admin revisa e aprova
4. Sugestões aprovadas → viram regras permanentes

**Estrutura de dados**:
```json
{
  "competency": "Comunicação Assertiva",
  "question": "Selecione os 3 que mais...",
  "validations": {
    "core": [
      {
        "type": "inversion",
        "counterQuestion": "Selecione os 3 que menos...",
        "source": "system"
      }
    ],
    "suggested": [
      {
        "type": "correlation",
        "counterQuestion": "Selecione os 3 que mais escutam ativamente",
        "confidence": 0.87,
        "source": "ai",
        "approved": false
      }
    ]
  }
}
```

**O que falta implementar**:
- Entidades: `QuestionTemplate`, `ComparisonQuestion`, `ValidationRule`, `CounterQuestion`
- Engine de conversão: ranking → scores normalizados
- Detector de inconsistências e viés
- Interface de aprovação de validações sugeridas por IA
- Banco de pares de validação core (competências correlatas)

**Próximo passo**:
- Especificar na camada PDI o motor de perguntas comparativas
- Definir na camada AI Copilot o sistema de validação híbrido
- Mapear pares de validação core para competências principais

---

