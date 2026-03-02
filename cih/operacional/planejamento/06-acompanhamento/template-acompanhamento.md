# Template - Acompanhamento Agentivo de Competências

## Protocolo do Agente de Acompanhamento

> Este documento define a lógica e os templates que o agente autônomo utiliza para acompanhar o desenvolvimento dos colaboradores.

---

### 1. Ciclo de Check-in (Quinzenal)

#### Mensagem de Check-in para o Colaborador

```
Olá [NOME]! Aqui é o acompanhamento do seu Plano de Desenvolvimento.

Vamos fazer um check rápido sobre suas ações das últimas 2 semanas?

COMPETÊNCIA: [COMPETÊNCIA_1]
Ação planejada: [AÇÃO]
→ Você conseguiu executar? (Sim / Parcialmente / Não)
→ Conte brevemente o que fez ou o que impediu:

COMPETÊNCIA: [COMPETÊNCIA_2]
Ação planejada: [AÇÃO]
→ Você conseguiu executar? (Sim / Parcialmente / Não)
→ Conte brevemente:

Alguma conquista que queira registrar?
Alguma dificuldade que precisa de apoio?

Obrigado pelo seu compromisso com o desenvolvimento!
```

#### Lógica de Processamento da Resposta

| Resposta | Ação do Agente |
|----------|---------------|
| Sim + descrição | Registra progresso. Mensagem de reconhecimento. |
| Parcialmente | Registra progresso parcial. Pergunta se precisa de ajuste na ação. |
| Não + justificativa | Registra bloqueio. Avalia se é recorrente. Sugere alternativa. |
| Não responde em 3 dias | Envia lembrete gentil. |
| Não responde em 7 dias | Alerta o consultor para intervenção. |
| 2+ "Não" consecutivos | Alerta o consultor. Possível revisão do PDI necessária. |

---

### 2. Relatório Mensal para o Consultor

```
RELATÓRIO MENSAL DE ACOMPANHAMENTO
Período: [MÊS/ANO]
Empresa: [EMPRESA]

RESUMO GERAL:
- Colaboradores ativos: [N]
- Taxa de resposta aos check-ins: [X]%
- Ações concluídas no período: [X] de [Y] ([Z]%)

POR COLABORADOR:

[NOME_1] - [CARGO]
  Status geral: [Verde/Amarelo/Vermelho]
  Competências:
    - [COMP_1]: Progresso [X]% | [DETALHES]
    - [COMP_2]: Progresso [X]% | [DETALHES]
  Alertas: [NENHUM / DESCRIÇÃO]
  Próximas ações: [LISTA]

[NOME_2] - [CARGO]
  Status geral: [Verde/Amarelo/Vermelho]
  ...

ALERTAS QUE REQUEREM INTERVENÇÃO:
  1. [COLABORADOR] - [MOTIVO] - [AÇÃO SUGERIDA]

MÉTRICAS DO PROGRAMA:
  - NPS do programa (se coletado): [VALOR]
  - Competências com maior evolução: [LISTA]
  - Competências com maior dificuldade: [LISTA]
```

---

### 3. Critérios de Alerta

#### Semáforo de Progresso

| Cor | Critério | Ação |
|-----|----------|------|
| Verde | 70%+ das ações executadas, check-ins respondidos | Manter ritmo |
| Amarelo | 40-69% das ações, ou 1 check-in perdido | Agente intensifica acompanhamento |
| Vermelho | <40% das ações, ou 2+ check-ins perdidos, ou bloqueios graves | Consultor intervém |

#### Triggers de Alerta ao Consultor

- Colaborador não responde há 7+ dias
- 2 ciclos consecutivos sem progresso
- Colaborador reporta conflito com liderança
- Colaborador pede para sair do programa
- Mudança de cargo/área do colaborador
- Desligamento iminente reportado

---

### 4. Registro de Evidências

| Data | Colaborador | Competência | Evidência | Fonte | Tipo |
|------|-------------|-------------|-----------|-------|------|
| [DATA] | [NOME] | [COMP] | [DESCRIÇÃO] | Check-in | Progresso |
| [DATA] | [NOME] | [COMP] | [DESCRIÇÃO] | Liderança | Feedback |
| [DATA] | [NOME] | [COMP] | [DESCRIÇÃO] | Auto-reporte | Conquista |

---

### 5. Mensagens Padrão do Agente

#### Reconhecimento de Conquista
```
Parabéns, [NOME]! Excelente progresso em [COMPETÊNCIA].
[DETALHES_DO_QUE_FOI_FEITO]
Continue assim! Próximo passo: [PRÓXIMA_AÇÃO]
```

#### Lembrete Gentil
```
Oi [NOME], tudo bem? Percebi que ainda não respondeu ao check-in.
Pode ser rápido - só preciso saber como estão suas ações de desenvolvimento.
Se estiver com dificuldade, me conte que podemos ajustar juntos.
```

#### Sugestão de Ajuste
```
[NOME], notei que a ação "[AÇÃO]" tem sido difícil de executar.
Que tal tentarmos uma abordagem diferente?
Opção A: [ALTERNATIVA_1]
Opção B: [ALTERNATIVA_2]
O que faz mais sentido pra você?
```

#### Alerta de Intervenção (para o consultor)
```
ALERTA - Intervenção necessária
Colaborador: [NOME]
Motivo: [MOTIVO]
Histórico recente: [RESUMO]
Ação sugerida: [SUGESTÃO]
Urgência: [Alta/Média]
```
