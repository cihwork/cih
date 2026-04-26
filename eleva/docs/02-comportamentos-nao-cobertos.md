# Comportamentos Não Cobertos - MCP-RH

Este documento detalha lacunas identificadas na documentação atual do MCP-RH, focando em casos de borda, fluxos alternativos, cenários de falha e requisitos não funcionais essenciais para a operação real do sistema.

## 1. Edge Cases (Casos de Borda)

- **Offboarding e Ciclo de Vida do Dado:** Não há definição do que ocorre com o histórico de PDI, avaliações DISC e evidências quando um colaborador é desligado. O dado é anonimizado, excluído ou arquivado para fins jurídicos?
- **Mudanças de Hierarquia (Gestão Matricial):** Como o sistema se comporta quando um colaborador possui dois líderes (funcional e projeto) ou quando muda de área no meio de um ciclo de PDI? As metas e históricos são transferidos ou "resetados"?
- **Identidades Duplicadas Cross-Tenant:** Cenário onde um consultor ou coach atua em múltiplos tenants. A documentação foca em isolamento, mas não explica a experiência de quem transita entre contas (leitura de dados, troca de contexto).
- **Assessments Incompletos:** Comportamento do sistema quando um DISC é iniciado e não finalizado, ou quando um PDI é criado sem metas vinculadas. Como isso afeta o "semáforo de saúde" do RH?

## 2. Fluxos Alternativos e Exceções

- **Revisão de Ciclo (Pivotagem):** Fluxo para quando as metas da empresa mudam drasticamente no meio do trimestre, invalidando os PDIs atuais. Existe um processo de "cancelamento em lote" ou "ajuste de rota"?
- **Feedback Contestados:** Mecanismo para quando o colaborador discorda de uma nota de competência ou de um registro de 1:1 feito pelo líder. Existe um fluxo de mediação pelo BP?
- **Revogação de Consentimento (LGPD):** Se um usuário retirar o opt-in para dados sensíveis comportamentais, como o sistema lida com os dados já coletados e com as recomendações de IA que já foram geradas?
- **Reversão de Promoções/Movimentações:** Impacto no histórico de competências quando uma mudança de cargo é revertida no sistema de origem (HRIS).

## 3. Integrações e Dependências

- **Falhas de Sincronização com HRIS:** O que acontece se o sistema de origem (ex: Totvs, Gupy, SAP) enviar dados inconsistentes ou se a integração falhar por dias? O MCP-RH opera em "modo offline" com dados em cache ou bloqueia ações?
- **Bias e Erros do Copilot de IA:** Fluxo de denúncia ou correção para recomendações de IA que sejam consideradas enviesadas, ofensivas ou tecnicamente incorretas.
- **Throttling e Cotas:** Limites de uso de tokens de IA por tenant ou limites de avaliações DISC simultâneas. Como o sistema avisa o Admin que o "limite metodológico" ou financeiro foi atingido?

## 4. Casos de Uso Complexos

- **Fusões e Aquisições (M&A):** Cenário de fusão de dois tenants. Como unificar frameworks de competências diferentes em uma única visão sem perder o histórico dos colaboradores?
- **Reestruturação Organizacional em Lote:** Movimentação massiva de 100+ colaboradores entre áreas. O sistema suporta updates via planilha/API ou exige movimentação individual?
- **Multi-idioma e Localização:** Como o PDI e as competências são exibidos para times globais? A IA traduz as recomendações ou elas são fixas por tenant?

## 5. Requisitos Não Funcionais (Ausentes)

- **Performance e Latência de IA:** Qual o tempo de resposta aceitável para a geração de um resumo de 1:1 ou recomendação de PDI?
- **Disponibilidade e SLA:** Garantia de uptime, especialmente em períodos críticos de fechamento de ciclo (final de trimestre/ano).
- **RTO e RPO (Disaster Recovery):** Qual a perda tolerável de dados humanos (evidências de PDI) em caso de falha catastrófica?
- **Exportabilidade de Dados (Lock-in):** O cliente pode exportar todo o histórico de desenvolvimento humano em formato aberto caso decida encerrar o contrato?
- **Auditoria de Consultas de IA:** Registro de quem consultou o quê via IA, para evitar que BPs ou líderes usem a ferramenta para bisbilhotar dados fora de sua alçada.
