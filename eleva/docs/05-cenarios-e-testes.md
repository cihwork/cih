# MCP-RH - Cenarios e Casos de Teste

**Objetivo:** documentar a estrategia de testes do MCP-RH com foco em cenarios completos, cobertura por modulo e validacoes criticas de negocio.

**Referencias usadas**

- [MCP-RH - Modulos e experiencia](/home/headless/workspace/projeto-sistema/mcp-rh/docs/03-modulos-e-experiencia.md)
- [MCP-RH - Arquitetura multi-tenant](/home/headless/workspace/projeto-sistema/mcp-rh/docs/04-arquitetura-multi-tenant.md)
- [MCP-RH - Visao do produto](/home/headless/workspace/projeto-sistema/mcp-rh/docs/02-visao-do-produto.md)
- [MCP-RH - Governanca](/home/headless/workspace/projeto-sistema/mcp-rh/docs/06-governanca.md)
- [Cadence - RBAC](/home/headless/workspace/dev/Cadence/docs/rbac.md)
- [Cadence - Setup e multi-tenancy](/home/headless/workspace/dev/Cadence/docs/setup.md)
- [Cadence - Modelo de dados de instancias](/home/headless/workspace/dev/Cadence/docs/reference/data-model/instances.sql.md)
- [Cadence - Modelo de dados de tasks](/home/headless/workspace/dev/Cadence/docs/reference/data-model/tasks.sql.md)
- [Cadence - Status completo do sistema](/home/headless/workspace/dev/Cadence/docs/status-completo.md)

## Premissas de referencia

O Cadence foi usado como referencia de estrutura e profundidade de testes por adotar:

- testes unitarios em servicos, validadores e formatadores;
- testes de integracao com banco real e fixture de contexto;
- testes E2E de fluxo completo com browser;
- cobertura de multi-tenancy por `InstanceId`;
- validacao de autorizacao via roles;
- cobranca de regras de recorrencia, estado e calculos derivados.

Para o MCP-RH, a documentacao abaixo espelha esse padrao e organiza os testes por:

- unidade de negocio;
- integracao de dominio e persistencia;
- API e contrato;
- multi-tenant;
- performance;
- regressao.

## Escopo por modulo

- **Identity**: autenticacao, autorizacao, papeis, claims, tenant resolution e auditabilidade.
- **People**: cadastro de pessoas, vinculos, lideranca, competencias basicas e status.
- **PDI**: plano de desenvolvimento, gaps, metas, checkpoints, evidencias e progresso.
- **Assessments**: DISC, calibracao comportamental e leitura de contexto.
- **Performance**: BSC, objetivos, metas, indicadores e consolidacao de progresso.
- **Engagement**: clima, energia, risco humano, pulsos e sinais de alerta.
- **AI Copilot**: recomendacoes, resumos, prompts operacionais e explicabilidade.
- **Integrations**: MCP, canais conversacionais, webhooks e servicos externos de IA.
- **Governance/Admin**: opt-in, auditoria, privacidade, configuracoes e limites por tenant.

## Matriz de testes

Legenda:

- **A** = cobertura alta
- **M** = cobertura media
- **B** = cobertura basica

| Modulo | Unitario | Integracao | API | Multi-tenant | Performance | Regresso |
|---|---:|---:|---:|---:|---:|---:|
| Identity | A | A | A | A | B | A |
| People | A | A | A | A | B | A |
| PDI | A | A | A | A | A | A |
| Assessments | A | M | A | A | M | A |
| Performance | A | A | A | A | A | A |
| Engagement | A | A | A | A | A | A |
| AI Copilot | A | A | A | A | M | A |
| Integrations | A | A | A | A | M | A |
| Governance/Admin | A | A | A | A | B | A |

## Casos de teste unitarios

### 1. Identity

#### UT-ID-01 - Login valida credenciais e aplica bloqueio por falhas
- **Given** um usuario valido e tentativas de login com senha invalida.
- **When** o usuario excede o limite de tentativas configurado.
- **Then** o sistema bloqueia o acesso, registra o evento e retorna erro de autenticacao sem vazar se o usuario existe.

#### UT-ID-02 - Role por papel e por contexto
- **Given** um usuario com role global e permissao contextual por tenant.
- **When** a regra de autorizacao e avaliada.
- **Then** o sistema concede acesso apenas se role, claim e tenant coincidirem com a politica esperada.

#### UT-ID-03 - Tenant resolution por contexto de autenticacao
- **Given** um usuario autenticado com tenant principal e tenant selecionado.
- **When** o contexto e reconstruido apos login.
- **Then** o tenant ativo deve ser o tenant selecionado e nao o tenant padrao se houver troca explicita.

#### UT-ID-04 - Validacao de senha e politicas minimas
- **Given** uma senha menor que o minimo ou sem complexidade requerida.
- **When** a validacao for executada.
- **Then** o sistema rejeita o cadastro/alteracao com mensagem de validacao consistente.

### 2. People

#### UT-PE-01 - Cadastro evita duplicidade por tenant
- **Given** uma pessoa ja cadastrada no mesmo tenant com documento ou email unico.
- **When** um novo cadastro com a mesma chave de negocio e solicitado.
- **Then** o sistema rejeita a duplicidade no tenant atual.

#### UT-PE-02 - Vinculo lider-colaborador e reciproca de hierarquia
- **Given** uma pessoa apontada como lider de um time.
- **When** o vinculo e salvo.
- **Then** o sistema valida se o lider existe, pertence ao tenant e nao cria ciclos hierarquicos invalidos.

#### UT-PE-03 - Inativacao preserva historico
- **Given** uma pessoa com PDI, avaliações e feedbacks associados.
- **When** a pessoa e inativada.
- **Then** os historicos continuam consultaveis e a pessoa deixa de aparecer em selecoes operacionais novas.

#### UT-PE-04 - Dados sensiveis exigem consentimento
- **Given** um campo classificado como sensivel.
- **When** o usuario tenta gravar sem opt-in.
- **Then** o sistema bloqueia ou mascara o registro conforme a politica de governanca.

### 3. PDI

#### UT-PDI-01 - Criacao de PDI exige competencia, gap e responsavel
- **Given** um colaborador com lacuna mapeada.
- **When** o PDI e criado sem competencia ou sem responsavel.
- **Then** a validacao falha e indica os campos obrigatorios.

#### UT-PDI-02 - Progresso nao pode ultrapassar o limite permitido
- **Given** uma meta de desenvolvimento com percentual atual.
- **When** o usuario tenta salvar progresso acima de 100% ou abaixo de 0%.
- **Then** o sistema rejeita o valor fora do intervalo.

#### UT-PDI-03 - Checkpoint so pode ser concluido em sequencia valida
- **Given** checkpoints com estados pendente, em andamento e concluido.
- **When** um checkpoint futuro e marcado antes do anterior.
- **Then** o sistema aplica a regra de sequenciamento e impede a transicao invalida.

#### UT-PDI-04 - Evidencia valida tipo, origem e contexto
- **Given** uma evidencia de texto, arquivo ou link.
- **When** o usuario envia formato inconsistente com o tipo.
- **Then** o sistema recusa o payload e preserva integridade da trilha.

### 4. Assessments

#### UT-AS-01 - Pontuacao DISC retorna perfil consistente
- **Given** respostas do assessment com pesos definidos por dimensao.
- **When** o calculo e executado.
- **Then** o perfil dominante e o secundario seguem a regra de pontuacao e desempate definida.

#### UT-AS-02 - Resposta fora do conjunto permitido e rejeitada
- **Given** uma pergunta com opcoes fechadas.
- **When** o usuario envia valor que nao pertence ao conjunto valido.
- **Then** a validacao falha com erro de contrato.

#### UT-AS-03 - Calibracao contextual respeita nivel de maturidade
- **Given** um contexto organizacional com nivel de maturidade informado.
- **When** a regra de calibracao e aplicada.
- **Then** a leitura comportamental nao pode ser usada como rotulo fixo e deve gerar saida explicavel.

### 5. Performance

#### UT-PR-01 - Objetivo calcula status a partir de progresso e prazo
- **Given** um objetivo com meta, valor atual e data limite.
- **When** o status e recalculado.
- **Then** o sistema classifica corretamente em track, risco ou fora da trilha.

#### UT-PR-02 - Indicador valida unidade e formula
- **Given** um indicador configurado com unidade de medida e formula.
- **When** a entrada de progresso nao bate com a unidade esperada.
- **Then** o sistema bloqueia a atualizacao ou converte apenas quando a regra permitir.

#### UT-PR-03 - Agregacao de BSC nao mistura periodos
- **Given** resultados de periodos distintos.
- **When** o consolidado e calculado.
- **Then** cada ciclo permanece isolado e a agregacao respeita a janela temporal.

### 6. Engagement

#### UT-EN-01 - Pulso calcula faixa de risco
- **Given** respostas de energia, carga e percepcao de apoio.
- **When** o score e calculado.
- **Then** o sistema classifica o resultado dentro da faixa esperada e nao gera falso positivo fora do limiar.

#### UT-EN-02 - Resposta anonima nao expõe identidade
- **Given** um fluxo configurado como anonimo.
- **When** a resposta e armazenada.
- **Then** a persistencia nao deve carregar identificadores pessoais indevidos.

#### UT-EN-03 - Alerta respeita nao punitividade
- **Given** um score de risco alto.
- **When** a recomendacao e gerada.
- **Then** a saida precisa ser de apoio, nao de punição.

### 7. AI Copilot

#### UT-AI-01 - Prompt contem contexto suficiente e explicavel
- **Given** um pedido de recomendacao.
- **When** o prompt e montado.
- **Then** a saida inclui contexto do colaborador, objetivo e restricoes de privacidade.

#### UT-AI-02 - Falha do provedor gera fallback previsivel
- **Given** o servico de IA indisponivel.
- **When** uma recomendacao e solicitada.
- **Then** o sistema retorna fallback sem quebrar o fluxo principal.

#### UT-AI-03 - Recomendacao nao pode ser emitida sem opt-in
- **Given** dados sensiveis sem consentimento.
- **When** o motor de IA e chamado.
- **Then** a operacao e bloqueada antes de enviar dados ao provedor externo.

### 8. Integrations

#### UT-IN-01 - Assinatura de webhook e validada
- **Given** um payload com assinatura.
- **When** a assinatura nao corresponde ao segredo esperado.
- **Then** o evento e rejeitado e nao segue para processamento.

#### UT-IN-02 - Idempotencia evita duplicidade
- **Given** um evento externo repetido com a mesma chave.
- **When** o segundo envio ocorre.
- **Then** o sistema nao cria duplicidade de evento, acao ou evidencia.

#### UT-IN-03 - Mapeamento de origem preserva rastreabilidade
- **Given** um evento vindo de IA, webhook ou conector MCP.
- **When** a ocorrencia e registrada.
- **Then** a origem e persistida de forma auditavel.

### 9. Governance/Admin

#### UT-GV-01 - Operacao sensivel exige auditoria
- **Given** uma alteracao de permissao ou dado sensivel.
- **When** a operacao e confirmada.
- **Then** o log de auditoria deve ser gerado com autor, tenant e timestamp.

#### UT-GV-02 - Escopo de tenant e aplicado em filtros
- **Given** duas contas cliente com registros equivalentes.
- **When** a consulta e executada.
- **Then** o retorno contem apenas dados do tenant atual.

## Casos de teste de integracao

Os cenarios abaixo devem usar banco real de teste, contexto persistido e injeção de dependencias completa, seguindo o padrao de Cadence com fixture e service provider.

### IT-01 - Onboarding completo de tenant
- **Given** uma nova instancia/tenant, um administrador e parametros iniciais.
- **When** o onboarding e executado.
- **Then** o tenant fica ativo, o usuario administrador recebe acesso, e os modulos base ficam disponiveis sem acessar dados de outros tenants.

### IT-02 - Pessoa -> PDI -> checkpoint -> evidencia
- **Given** uma pessoa com gap de competencia e um PDI em aberto.
- **When** o usuario cria checkpoint, registra evidencia e conclui uma etapa.
- **Then** o progresso e atualizado no banco, a trilha fica auditavel e o historico mostra a sequencia correta.

### IT-03 - Avaliacao DISC alimenta recomendacao de lideranca
- **Given** um assessment concluido.
- **When** o sistema consolida o perfil e gera uma recomendacao.
- **Then** a recomendacao incorpora o resultado da avaliacao e respeita o contexto do tenant e do perfil do usuario.

### IT-04 - Objetivo de performance recalcula status apos atualizacao
- **Given** um objetivo em andamento com progresso parcial.
- **When** o progresso e alterado e o job de consolidacao executa.
- **Then** o status final e reprocessado e persistido de forma consistente.

### IT-05 - Pulsos de engagement atualizam visao consolidada
- **Given** multiplas respostas de pulso de um time.
- **When** o consolidado e recalculado.
- **Then** o semaforo de risco humano e o resumo do time refletem os novos dados.

### IT-06 - IA externa com timeout e fallback
- **Given** um provedor de IA lento ou indisponivel.
- **When** a recomendacao e solicitada.
- **Then** o sistema registra erro controlado, aciona fallback e nao interrompe o fluxo do usuario.

### IT-07 - Persistencia com isolamento de tenant
- **Given** dois tenants com cadastros parecidos.
- **When** o fluxo grava pessoa, PDI e recomendacoes em ambos.
- **Then** cada conjunto de registros permanece restrito ao tenant de origem.

### IT-08 - Reprocessamento por evento externo
- **Given** um webhook ou evento MCP valido.
- **When** o evento e persistido e consumido.
- **Then** o sistema atualiza o estado de negocio esperado e evita processamento duplicado.

## Casos de teste de API

Como o contrato final de endpoints do MCP-RH ainda nao esta fechado nos docs de referencia, os casos abaixo estao descritos por operacao de API esperada. No momento de implementar, cada caso deve ser mapeado para o endpoint REST efetivo e para o schema OpenAPI final.

### API-01 - Autenticacao exige credenciais validas
- **Given** um corpo de requisicao sem credenciais ou com credenciais invalidas.
- **When** a operacao de login e chamada.
- **Then** a API retorna 401 ou 400, sem indicar detalhes internos.

### API-02 - Autorizacao bloqueia acesso fora do papel
- **Given** um token valido sem permissao para modulos administrativos.
- **When** a rota administrativa e chamada.
- **Then** a API retorna 403 e nao expõe dados sensiveis.

### API-03 - Criacao de pessoa valida contrato
- **Given** um payload de pessoa com campo obrigatorio ausente, formato invalido ou tenant incorreto.
- **When** a API de criacao e chamada.
- **Then** a resposta retorna 400 com lista de erros de validacao e sem persistir nada.

### API-04 - Listagem respeita paginação e filtros
- **Given** uma requisicao com pagina, tamanho e filtros por status ou area.
- **When** a consulta e executada.
- **Then** o contrato retorna metadados de paginação, itens filtrados e ordem previsivel.

### API-05 - Atualizacao nao permite troca indevida de tenant
- **Given** um recurso existente em um tenant e um token de outro tenant.
- **When** a API de update e chamada com identificador externo valido.
- **Then** a API retorna 404 ou 403 conforme a politica, sem permitir vazamento cruzado.

### API-06 - PDI nao aceita progresso fora da faixa
- **Given** um payload com percentual negativo ou acima do limite.
- **When** a API de atualizacao de progresso e chamada.
- **Then** a resposta e 400 com detalhe de campo.

### API-07 - Endpoint de recomendacao exige escopo e consentimento
- **Given** uma chamada para gerar recomendacao com dados sem opt-in.
- **When** a API de AI Copilot e chamada.
- **Then** a operacao e recusada antes de enviar dados ao provedor externo.

### API-08 - Webhook valida assinatura e idempotencia
- **Given** um evento recebido com assinatura valida.
- **When** o mesmo evento chega novamente.
- **Then** a primeira requisicao e aceita e a segunda e tratada como idempotente.

### API-09 - Contrato retorna erros padronizados
- **Given** uma falha de dominio, validacao ou autorizacao.
- **When** a API responde.
- **Then** o formato de erro segue padrao unico, com codigo, mensagem e detalhes de campo.

### API-10 - Download de relatorio respeita tenant e permissao
- **Given** um usuario com acesso ao modulo, mas sem permissao de exportacao.
- **When** o endpoint de exportacao e chamado.
- **Then** a API nega acesso e nao gera arquivo.

## Casos de teste multi-tenant

### MT-01 - Isolamento total de dados
- **Given** dois tenants com pessoas, PDIs e avaliacoes identicos.
- **When** um usuario consulta dados no tenant A.
- **Then** apenas registros do tenant A sao retornados.

### MT-02 - Filtro por tenant em consultas derivadas
- **Given** uma query consolidada com joins entre pessoas, PDI e performance.
- **When** a consulta e executada.
- **Then** todos os joins preservam o filtro por tenant sem misturar registros.

### MT-03 - Tenant switching limpa contexto
- **Given** um usuario com acesso a mais de um tenant.
- **When** ele troca de tenant na interface ou por contexto de sessao.
- **Then** as proximas consultas usam o tenant novo e nao reutilizam cache do tenant anterior.

### MT-04 - Reuso do mesmo usuario em tenants diferentes
- **Given** o mesmo usuario vinculado a dois tenants.
- **When** ele acessa cada tenant em momentos distintos.
- **Then** o sistema respeita permissao e perfil em cada contexto sem sobrescrever historico.

### MT-05 - Operacoes administrativas com escopo global nao vazam dados
- **Given** um admin com visao consolidada.
- **When** ele consulta relatorios cross-tenant.
- **Then** o sistema somente libera agregados permitidos pela politica e nunca registros brutos sem autorizacao.

### MT-06 - Workflows automatizados respeitam InstanceId
- **Given** jobs, integrações ou consumers automáticos.
- **When** eles criam ou atualizam registros.
- **Then** o `InstanceId` correto e gravado e a entidade nao aparece em outro tenant.

## Casos de teste de performance

Os testes de performance devem ser executados com dados representativos e com medicao de latencia, throughput e consumo de memoria.

### PF-01 - Lista de pessoas em tenant grande
- **Given** um tenant com volume alto de colaboradores e historico.
- **When** a listagem e filtragem sao executadas.
- **Then** a resposta deve permanecer dentro do SLA definido para consulta paginada.

### PF-02 - Criacao em lote de PDIs
- **Given** dezenas ou centenas de PDIs em lote.
- **When** a operacao de criacao e executada.
- **Then** o sistema nao degrada de forma nao linear nem gera timeouts.

### PF-03 - Pulsos simultaneos
- **Given** um pulso aberto para varios times ao mesmo tempo.
- **When** respostas concorrentes chegam em janela curta.
- **Then** o consolidado deve manter consistencia e evitar perda de escrita.

### PF-04 - Recomendacao de IA sob carga
- **Given** multiplos pedidos de recomendacao em paralelo.
- **When** o provedor externo responde com latencia variavel.
- **Then** o sistema deve aplicar timeout, fila ou fallback sem bloquear o fluxo principal.

### PF-05 - Filtro multi-tenant em consultas pesadas
- **Given** um banco com muitos tenants e registros historicos.
- **When** consultas cruzadas com filtro por tenant sao executadas.
- **Then** o plano de execucao nao deve degradar por falta de indice ou filtro ineficiente.

### PF-06 - Stress em auditoria e logs
- **Given** operacoes sensiveis em sequencia.
- **When** as trilhas de auditoria sao gravadas.
- **Then** o volume de logs nao pode comprometer o tempo de resposta de operacoes transacionais.

## Testes de regressao

Os cenarios abaixo nao podem quebrar em nenhuma liberacao.

### RG-01 - Login, selecao de tenant e acesso inicial
- **Given** um usuario valido.
- **When** ele autentica e escolhe o tenant.
- **Then** o sistema deve abrir a sessao correta e carregar somente dados daquele tenant.

### RG-02 - Cadastro de pessoa com lideranca
- **Given** um novo colaborador e um lider atribuido.
- **When** o cadastro e salvo.
- **Then** a pessoa deve aparecer no organograma e nas visoes de time corretas.

### RG-03 - PDI com checkpoint e evidencia
- **Given** um plano ativo.
- **When** checkpoint e evidencia sao adicionados e atualizados.
- **Then** o historico deve continuar consistente e o progresso nao pode retroceder indevidamente.

### RG-04 - Avaliacao DISC nao vira rotulo fixo
- **Given** um perfil comportamental concluido.
- **When** a recomendacao e exibida.
- **Then** o texto deve ser interpretativo e nao deterministico ou estigmatizante.

### RG-05 - Consolidado de performance permanece correto
- **Given** objetivos, metas e indicadores.
- **When** novos valores sao gravados.
- **Then** o percentual e o status derivados continuam corretos.

### RG-06 - Alertas de engagement permanecem orientados a apoio
- **Given** um time com sinais de risco.
- **When** o dashboard e recalculado.
- **Then** a prioridade continua sendo orientacao e acompanhamento, nao penalizacao.

### RG-07 - Integracao externa nao quebra fluxo interno
- **Given** falha temporaria em IA, webhook ou MCP.
- **When** um usuario continua operando o sistema.
- **Then** os fluxos internos essenciais permanecem funcionando.

## Cenarios de integracao prioritarios

### Fluxo 1 - Onboarding de tenant e primeiro ciclo de pessoas

1. Criar tenant.
2. Provisionar admin e politicas basicas.
3. Cadastrar time, lider e colaboradores.
4. Registrar assessment inicial.
5. Abrir PDI de desenvolvimento.
6. Gerar primeira recomendacao assistida por IA.

**Critério de sucesso**

- o tenant fica isolado;
- o admin acessa somente seu escopo;
- o PDI nasce com contexto correto;
- a recomendacao e explicavel e auditavel.

### Fluxo 2 - Desenvolvimento continuo com checkpoint

1. Identificar gap de competencia.
2. Criar plano de desenvolvimento.
3. Agendar checkpoint.
4. Registrar evidencia.
5. Atualizar progresso.
6. Revisar recomendacao.

**Critério de sucesso**

- a trilha fica cronologica;
- o progresso se mantem coerente;
- a evidencia fica associada ao tenant e a pessoa corretos.

### Fluxo 3 - Performance e risco humano

1. Cadastrar objetivo e indicadores.
2. Receber pulsos de engagement.
3. Recalcular semaforo de risco.
4. Consolidar status de performance.
5. Emitir alerta de apoio ao lider.

**Critério de sucesso**

- os calculos derivam dos dados persistidos;
- o alerta nao expõe dados alem do necessario;
- o dashboard nao mistura tenants.

### Fluxo 4 - Integração externa com IA

1. Receber evento externo ou solicitação de analise.
2. Validar assinatura/contrato.
3. Montar contexto minimizado.
4. Chamar o provedor de IA.
5. Persistir a resposta com auditoria.

**Critério de sucesso**

- a entrada e validada antes do consumo;
- o provedor recebe apenas dados permitidos;
- a resposta fica rastreavel e reproduzivel.

## Critérios de aceitação

- Cada modulo critico precisa ter pelo menos um caso unitario por regra de negocio principal.
- Cada fluxo fim a fim precisa ser coberto por integracao com banco real.
- Toda operacao sensivel deve ter teste de autorizacao e auditoria.
- Toda query de dominio precisa comprovar isolamento por tenant.
- Toda saida de IA precisa comprovar explicabilidade e respeito ao opt-in.
- Toda API precisa documentar validacao de contrato, erro padrao e seguranca.
- Todo caso de regressao precisa ser executado em cada entrega relevante.

## Prioridade de execucao sugerida

1. Identity e multi-tenant.
2. People e PDI.
3. Performance e engagement.
4. Assessments e AI Copilot.
5. Integrations e governanca.
