# Gaps, inconsistências e problemas semânticos do MCP-RH

Análise baseada exclusivamente nos documentos em `/home/headless/workspace/projeto-sistema/mcp-rh/docs/`.

Escopo do diagnóstico:
- `00-resumo-executivo.md`
- `01-fonte-e-problema.md`
- `02-visao-do-produto.md`
- `03-modulos-e-experiencia.md`
- `04-arquitetura-multi-tenant.md`
- `05-roadmap.md`
- `06-governanca.md`

## Sumário executivo

A documentação descreve bem a tese do produto, os perfis de uso e a intenção arquitetural. Porém, ainda está em um nível conceitual alto demais para sustentar decisões de produto, arquitetura e governança sem interpretações adicionais.

Os principais problemas são:
- conceitos centrais não definidos, especialmente `EC`, `risco humano`, `saúde humana`, `engajamento do líder` e `semáforo`;
- ausência de fluxos operacionais para consentimento, auditoria, onboarding, avaliação, PDI, feedback e IA;
- nomenclatura inconsistente para os mesmos eixos de negócio;
- premissas fortes sobre comportamento humano e eficácia metodológica sem validação explícita.

Em termos de prioridade, os gaps mais críticos estão em:
1. definição do modelo conceitual e semântico do produto;
2. governança de dados sensíveis e papéis;
3. desenho dos fluxos operacionais de PDI, check-ins, DISC, EC e IA;
4. integração com o ecossistema corporativo existente;
5. critérios mensuráveis para KPIs e alertas.

## 1. Lista categorizada de gaps

### 1.1 Gaps de produto e escopo

1. Não existe definição operacional do que é `EC`.
   - Evidência: `00-resumo-executivo.md:10`, `02-visao-do-produto.md:3`, `03-modulos-e-experiencia.md:8`, `04-arquitetura-multi-tenant.md:21`, `05-roadmap.md:24-28`.
   - Lacuna: o texto alterna entre `EC`, `nível de consciência organizacional`, `calibração cultural` e `maturidade` sem esclarecer se são sinônimos, camadas de um mesmo modelo ou conceitos distintos.
   - Impacto: impede desenho de domínio, formulários, escalas, regras e relatórios.

2. O escopo do produto não distingue claramente o que entra no MCP-RH e o que fica fora dele.
   - Evidência: `00-resumo-executivo.md:15`, `02-visao-do-produto.md:7-14`, `04-arquitetura-multi-tenant.md:37-39`.
   - Lacuna: a documentação afirma que o produto não compete com HRIS, mas não define integrações mínimas, fronteiras funcionais ou dependências externas.
   - Impacto: risco de escopo elástico, duplicação funcional e expectativas desalinhadas.

3. Não há decisão explícita sobre integração com sistemas corporativos existentes.
   - Evidência: `02-visao-do-produto.md:7-14`, `04-arquitetura-multi-tenant.md:19-26`, `05-roadmap.md:31-36`.
   - Lacuna: faltam posições sobre SSO, HRIS, folha, calendário, e-mail, mensageria, storage documental e canais conversacionais.
   - Impacto: a arquitetura fica incompleta e o roadmap de implementação perde prioridade real.

4. A documentação não define o que constitui sucesso do produto em termos operacionais.
   - Evidência: `06-governanca.md:28-30`.
   - Lacuna: o critério de sucesso é textual e aspiracional, mas não há métricas-alvo, baseline, período de observação ou responsabilidade por medição.
   - Impacto: dificulta validação de produto e governança de roadmap.

### 1.2 Gaps de processo

1. Falta o fluxo completo de onboarding do tenant.
   - Evidência: `04-arquitetura-multi-tenant.md:9,19,30-31`, `05-roadmap.md:3-8,38-44`.
   - Lacuna: não há etapas de provisionamento, configuração inicial, importação de dados, convites, aceites e publicação de política de uso.
   - Impacto: a primeira experiência do cliente fica indefinida.

2. Falta o fluxo de consentimento e revogação para dados sensíveis.
   - Evidência: `04-arquitetura-multi-tenant.md:31`, `06-governanca.md:5-9`.
   - Lacuna: a documentação cita opt-in, mas não define quando coletar, como armazenar, como revogar, como auditar e o que acontece com dados já coletados.
   - Impacto: risco jurídico, de compliance e de implementação.

3. Não existe lifecycle documentado para PDI.
   - Evidência: `00-resumo-executivo.md:7,20`, `01-fonte-e-problema.md:25-29`, `03-modulos-e-experiencia.md:10,26-28`, `05-roadmap.md:10-15`.
   - Lacuna: faltam criação, aprovação, revisão, atualização, encerramento, reabertura, status e evidências mínimas.
   - Impacto: o principal módulo do produto fica sem estrutura de operação.

4. Não há fluxo de check-in, 1:1 e feedback.
   - Evidência: `03-modulos-e-experiencia.md:11,30-34`, `05-roadmap.md:17-22,43`.
   - Lacuna: não se sabe periodicidade, template, responsáveis, SLA, artefatos gerados e vínculo com PDI ou metas.
   - Impacto: a proposta de “liderança assistida” não é implementável de forma consistente.

5. Não há fluxo para recomendações assistidas por IA.
   - Evidência: `00-resumo-executivo.md:11`, `03-modulos-e-experiencia.md:13`, `04-arquitetura-multi-tenant.md:25,32`, `05-roadmap.md:31-36`.
   - Lacuna: não existe definição sobre origem dos dados, critérios de confiança, necessidade de revisão humana, logging, rastreabilidade do prompt e tratamento de falhas.
   - Impacto: alto risco de opacidade, excesso de confiança e problemas de auditoria.

6. Não há um processo claro de avaliação e evolução de competências.
   - Evidência: `02-visao-do-produto.md:13`, `03-modulos-e-experiencia.md:10,21`, `05-roadmap.md:13-15`.
   - Lacuna: a documentação cita competências, gaps, progresso e checkpoints, mas não explica como a competência é criada, escalada, avaliada, calibrada ou encerrada.
   - Impacto: inviabiliza padronização metodológica.

### 1.3 Gaps de dados e governança

1. Não há matriz de papéis e permissões.
   - Evidência: `02-visao-do-produto.md:16-24`, `04-arquitetura-multi-tenant.md:12,19-33`.
   - Lacuna: os perfis existem, mas não há definição de permissões por ação, por dado, por contexto e por tenant.
   - Impacto: a regra de “acesso mínimo necessário” não pode ser implementada de forma verificável.

2. Não há política de retenção, expurgo, exportação ou portabilidade de dados.
   - Evidência: `04-arquitetura-multi-tenant.md:31`, `06-governanca.md:5-9`.
   - Lacuna: a documentação trata dados sensíveis, mas não fala em ciclo de vida, retenção, deleção, exportação, anonimização ou backup.
   - Impacto: grande exposição em LGPD e em auditorias internas.

3. Não há classificação formal do que são `dados comportamentais`, `dados de desempenho` e `dados humanos sensíveis`.
   - Evidência: `04-arquitetura-multi-tenant.md:31`, `06-governanca.md:5-6`.
   - Lacuna: a separação existe como princípio, mas não como taxonomia implementável.
   - Impacto: o produto pode tratar dados diferentes como se fossem equivalentes.

4. Não há definição de trilha de auditoria.
   - Evidência: `04-arquitetura-multi-tenant.md:11,31-33`, `06-governanca.md:8`.
   - Lacuna: não se sabe o que é auditado, quem consulta, por quanto tempo, nem como o usuário acessa o histórico.
   - Impacto: a exigência de rastreabilidade fica declarativa.

### 1.4 Gaps de métrica e validação

1. Os KPIs do produto estão nomeados, mas não definidos.
   - Evidência: `06-governanca.md:18-26`.
   - Lacuna: não há fórmula, fonte, periodicidade, owner, meta ou interpretações para `aderência ao PDI`, `completude de 1:1`, `score de engajamento do líder` e `nível de risco humano`.
   - Impacto: impossível monitorar evolução de produto com consistência.

2. Não existe definição dos semáforos citados na documentação.
   - Evidência: `03-modulos-e-experiencia.md:22`, `02-visao-do-produto.md:22`, `06-governanca.md:26`.
   - Lacuna: não se sabe se o semáforo é por pessoa, time, líder, tenant ou processo; também não se define cor, critério e ação associada.
   - Impacto: o mesmo termo pode ser interpretado de várias formas.

3. A noção de `risco humano` está sem métrica.
   - Evidência: `00-resumo-executivo.md:19`, `02-visao-do-produto.md:20`, `03-modulos-e-experiencia.md:22`, `06-governanca.md:25`.
   - Lacuna: não há modelo de cálculo, variáveis, limiares ou justificativa.
   - Impacto: risco de subjetividade e de uso inadequado em decisões de pessoas.

## 2. Inconsistências encontradas

### 2.1 Nomenclatura do eixo EC / maturidade / consciência

- `00-resumo-executivo.md:10` fala em `EC e maturidade como calibração de contexto`.
- `02-visao-do-produto.md:12` fala em `calibração de liderança por maturidade`.
- `03-modulos-e-experiencia.md:8` fala em `Consciência organizacional e calibração cultural`.
- `04-arquitetura-multi-tenant.md:21` fala em `Assessments | DISC, EC e leitura de contexto`.
- `05-roadmap.md:24-29` fala em `DISC, EC e calibração` e `nível de consciência organizacional`.

Problema:
- o mesmo eixo aparece com quatro rótulos diferentes;
- não fica claro se `EC` é o nome do modelo, se `maturidade` é atributo dele ou se `consciência organizacional` é uma tradução operacional.

Impacto:
- dificulta modelagem de domínio;
- gera risco de documentação duplicada ou contraditória;
- impede que times de produto, design e engenharia falem a mesma língua.

### 2.2 Nomenclatura de persona e papel

- `00-resumo-executivo.md:19-21` usa `BP`, `coach` e `líder`.
- `02-visao-do-produto.md:20-24` usa `BP / RH estratégico`, `Coach / consultor`, `Líder`, `Colaborador`, `Admin da conta`.
- `03-modulos-e-experiencia.md:18-24` usa `BP / RH` e `Coach / consultor`.

Problema:
- os nomes alternam entre papel organizacional, função operacional e perfil de uso;
- não existe um glossário que diga se `BP / RH` e `BP / RH estratégico` são a mesma persona;
- não está claro se `coach` e `consultor` são sinônimos, subtipos ou públicos distintos.

Impacto:
- dificulta desenho de permissões, jornadas e comunicação;
- gera sobreposição de responsabilidades entre perfis.

### 2.3 Semáforo de saúde humana versus semáforo executivo

- `03-modulos-e-experiencia.md:22` fala em `semáforo de saúde humana`;
- `02-visao-do-produto.md:22` fala em `semáforo de equipe`;
- `06-governanca.md:26` fala em `evolução do semáforo executivo`.

Problema:
- o artefato semáforo muda de alvo, nível e propósito ao longo dos documentos;
- não há garantia de que seja o mesmo objeto com nomes diferentes.

Impacto:
- alto risco de implantar dashboards diferentes para o que pode ser uma única lógica;
- muita margem para interpretação subjetiva.

### 2.4 PDI contínuo versus ciclo de PDI

- `00-resumo-executivo.md:7` afirma `PDI contínuo, não anual`;
- `01-fonte-e-problema.md:25-26` fala em `acompanhamento semanal ou quinzenal` e em `o ciclo`;
- `05-roadmap.md:12-15` fala em `ciclo de PDI` e `checkpoints e evidências`.

Problema:
- não há reconciliação entre continuidade, ciclo e cadência de revisão;
- a palavra `ciclo` pode sugerir fechamento periódico, enquanto o resumo enfatiza continuidade.

Impacto:
- pode haver implantações conflitantes sobre periodicidade, encerramento e reinício do PDI.

### 2.5 Linguagem de risco e saúde sem padronização

- `00-resumo-executivo.md:19` usa `risco humano`;
- `03-modulos-e-experiencia.md:12,22` usa `risco` e `saúde humana`;
- `04-arquitetura-multi-tenant.md:24` usa `humor, energia e sinais`;
- `06-governanca.md:25` usa `nível de risco humano por time`.

Problema:
- os termos parecem próximos, mas não têm definição comum;
- `humor` e `energia` são especialmente ambíguos e podem ser lidos como indicadores subjetivos ou até sensíveis.

Impacto:
- dificulta medição e explicabilidade;
- aumenta risco de interpretação indevida em contexto de pessoas.

## 3. Problemas semânticos críticos

### 3.1 Termos não definidos

Termos que aparecem como centrais, mas não têm definição operacional:
- `EC`
- `maturidade`
- `consciência organizacional`
- `calibração cultural`
- `risco humano`
- `saúde humana`
- `engajamento do líder`
- `semáforo executivo`
- `semáforo de saúde humana`
- `dados humanos sensíveis`
- `leitura humana`
- `calibração por contexto`

Consequência:
- a documentação usa esses termos como se fossem autoexplicativos, mas eles são interpretáveis de múltiplas formas.

### 3.2 Suposições implícitas que viram regra

1. `PDI precisa ser vivo, com acompanhamento semanal ou quinzenal`.
   - Evidência: `01-fonte-e-problema.md:25`.
   - Problema: a frequência é tratada como premissa, sem segmentação por tipo de cargo, área, criticidade ou maturidade.

2. `DISC ajuda a calibrar comunicação e risco de fricção`.
   - Evidência: `01-fonte-e-problema.md:27`, `00-resumo-executivo.md:8`.
   - Problema: o texto assume utilidade e validade prática sem explicitar limites, validade do instrumento ou forma de uso.

3. `O nível de consciência da empresa muda a forma de aplicar o PDI`.
   - Evidência: `01-fonte-e-problema.md:28`, `05-roadmap.md:27-29`.
   - Problema: a relação entre consciência/maturidade e desenho do PDI é afirmada, mas não descrita.

4. `O gestor precisa de recomendações práticas, não apenas dashboards`.
   - Evidência: `01-fonte-e-problema.md:29`.
   - Problema: isso orienta o produto, mas não define como recomendações serão validadas, priorizadas e entregues.

### 3.3 Lógica circular

Existe um ciclo conceitual não resolvido:
- o produto é calibrado por `maturidade` e `nível de consciência`;
- essa maturidade influencia a aplicação do PDI;
- o PDI e os sinais do sistema passam a ser usados para inferir evolução;
- a própria evolução do sistema vira critério de sucesso.

Referências:
- `00-resumo-executivo.md:10,19-22`
- `01-fonte-e-problema.md:28`
- `02-visao-do-produto.md:12,20-23`
- `06-governanca.md:20-30`

Problema:
- o documento usa variáveis de contexto para justificar a forma de operar o próprio sistema, mas não define como essas variáveis são medidas antes de o sistema existir.

### 3.4 Ambiguidade de causalidade

O texto sugere causalidade em vários pontos sem explicitar evidência ou mecanismo:
- `DISC` alterando comunicação e fricção;
- `EC / maturidade` alterando a forma de aplicar o PDI;
- `IA` melhorando consistência metodológica;
- `alertas` melhorando leitura humana sem virar mecanismo punitivo.

Referências:
- `00-resumo-executivo.md:8,11`
- `01-fonte-e-problema.md:27-29`
- `04-arquitetura-multi-tenant.md:13,32-33`

Problema:
- há uma sequência de promessa causal, mas sem critérios de validação.

## 4. Premissas não validadas

### 4.1 Premissas sobre comportamento do usuário

1. Líderes aceitarão usar 1:1 guiado, pauta pronta e recomendações curtas.
   - Evidência: `03-modulos-e-experiencia.md:30-34`, `05-roadmap.md:19-22`.
   - Risco: sem validação de adoção, o módulo pode virar mais uma obrigação operacional.

2. Colaboradores registrarão evidências e aprendizados de forma consistente.
   - Evidência: `03-modulos-e-experiencia.md:36-40`.
   - Risco: depende de motivação, tempo e percepção de justiça.

3. BP / RH usará o sistema para leitura de carteira e risco humano.
   - Evidência: `00-resumo-executivo.md:19`, `03-modulos-e-experiencia.md:18-22`.
   - Risco: a utilidade depende de confiabilidade dos sinais e clareza das regras.

### 4.2 Premissas metodológicas

1. DISC é um bom eixo operacional para ajuste de comportamento.
   - Evidência: `00-resumo-executivo.md:8`, `01-fonte-e-problema.md:27`.
   - Risco: a documentação trata o método como fundamento, mas não valida sua adequação ao contexto do produto.

2. Maturidade / consciência organizacional são eixos adequados para calibrar liderança.
   - Evidência: `00-resumo-executivo.md:10`, `02-visao-do-produto.md:12`, `05-roadmap.md:27-29`.
   - Risco: sem modelo mensurável, essa base conceitual fica difícil de operacionalizar.

3. `70-20-10` é um modelo aplicável ao PDI do produto.
   - Evidência: `05-roadmap.md:12-15`.
   - Risco: a documentação assume o framework sem discutir restrições, adaptação ou contexto de uso.

### 4.3 Premissas técnicas e operacionais

1. IA poderá atuar como copiloto sem ser decisora.
   - Evidência: `00-resumo-executivo.md:11`, `04-arquitetura-multi-tenant.md:13,25,32-33`.
   - Risco: faltam limites claros de intervenção, fallback humano e critérios de confiança.

2. O modelo multi-tenant resolverá os problemas de segregação e governança.
   - Evidência: `02-visao-do-produto.md:3`, `04-arquitetura-multi-tenant.md:9,30-31`.
   - Risco: a intenção está clara, mas a implementação de isolamento e autorização não está descrita.

3. Alertas podem ser usados com segurança sem se tornarem punitivos.
   - Evidência: `04-arquitetura-multi-tenant.md:33`, `06-governanca.md:13-16`.
   - Risco: sem regras de uso, o sistema pode ser apropriado para controle disciplinar.

## 5. Recomendações de correção priorizadas

### Prioridade 1 - Definir o modelo conceitual central

1. Criar um glossário oficial do MCP-RH.
   - Incluir definições para `EC`, `maturidade`, `consciência organizacional`, `risco humano`, `saúde humana`, `engajamento`, `semáforo`, `check-in`, `1:1`, `PDI`, `competência`, `gap` e `alerta`.
   - Objetivo: eliminar ambiguidades e padronizar linguagem entre produto, UX, engenharia e governança.

2. Escolher um nome único para o eixo de contexto organizacional.
   - Decidir se o termo primário será `EC`, `consciência organizacional` ou `maturidade`.
   - Objetivo: evitar documentação paralela para a mesma ideia.

3. Separar claramente persona, papel e permissão.
   - Explicitar se `BP / RH`, `BP / RH estratégico`, `coach` e `consultor` são sinônimos ou perfis distintos.
   - Objetivo: suportar matriz de acesso, jornada e comunicação.

### Prioridade 2 - Fechar os fluxos críticos

4. Documentar o ciclo de vida completo de PDI.
   - Descrever criação, alinhamento, revisão, evidência, encerramento, reabertura e auditoria.
   - Objetivo: transformar PDI de conceito em processo operável.

5. Documentar o fluxo de check-in, feedback e 1:1.
   - Especificar periodicidade, pauta, responsáveis, artefatos, lembretes e vínculo com metas e competências.
   - Objetivo: tornar a liderança assistida consistente e auditável.

6. Documentar o fluxo de consentimento e gestão de dados sensíveis.
   - Incluir opt-in, revogação, retenção, expurgo, exportação e trilha de auditoria.
   - Objetivo: reduzir risco jurídico e de implementação.

### Prioridade 3 - Operacionalizar métricas e alertas

7. Definir fórmulas e cadência dos KPIs.
   - Criar especificação para cada indicador listado em `06-governanca.md:18-26`.
   - Objetivo: permitir acompanhamento real do produto.

8. Definir semáforos com critérios objetivos.
   - Determinar escopo, cores, limiares, explicação e ação recomendada para cada semáforo.
   - Objetivo: evitar dashboards subjetivos e interpretações conflitantes.

9. Definir o modelo de `risco humano`.
   - Especificar sinais de entrada, agregação, limites e revisão humana obrigatória.
   - Objetivo: impedir uso opaco ou excessivamente sensível.

### Prioridade 4 - Completar arquitetura e governança

10. Criar a matriz de permissões por papel e contexto.
    - Cobrir leitura, escrita, aprovação, auditoria, exportação e uso de IA.
    - Objetivo: tornar aplicável o princípio de acesso mínimo necessário.

11. Documentar a estratégia de integração.
    - Definir dependências mínimas: SSO, HRIS, e-mail, calendário, armazenamento e canais.
    - Objetivo: evitar que o produto seja planejado como ilha.

12. Documentar o contrato de uso da IA.
    - Incluir limites, revisão humana, rastreabilidade e comportamento em caso de baixa confiança.
    - Objetivo: reduzir risco de automação indevida e de recomendações não explicáveis.

## 6. Conclusão

A documentação atual já sustenta a visão do produto, mas ainda não fecha os elementos necessários para execução segura e consistente.

O principal problema não é falta de ambição. É falta de definição operacional dos conceitos centrais e dos fluxos que conectam visão, governança e implementação.

Antes de avançar para desenho detalhado de produto ou arquitetura, o material precisa resolver:
- o significado exato de `EC` e dos termos relacionados;
- o modelo de permissões e consentimento;
- os fluxos de PDI, 1:1, feedback e IA;
- os critérios mensuráveis de sucesso e risco.

Sem isso, o produto corre o risco de nascer conceitualmente forte, mas semanticamente instável.
