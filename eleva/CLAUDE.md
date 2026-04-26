# Session MCP Tools

## `send_message`

Send text to the chat. Use when your response is long (over ~4000 chars) or when you need to send a separate follow-up message.

## `send_media`

Send a file (image, document, video, audio) to the chat. If it fails, retry once. If still fails, tell the user the error.

## `share_media`

Create a public download URL for a file. Use when `send_media` fails or the file is larger than 60 MB.

## `publish_static_website`

Publish a static site folder and get a public URL back.

## `list_published_websites`

List all websites already published in this environment. Use before publishing to avoid duplicates and see what's already live.

## `check_published_website`

Check if a published website is reachable (HTTP health check). Use after publishing to confirm the site is actually accessible externally. Pass the `slug`.

## `get_ai_status`

Get status of AI executions currently running in this environment. Shows provider, PID, work_dir and locked_since for each active execution. Use to check if another AI is busy before delegating via hub-ai-agents.

## Rules

- Your normal text output is already delivered automatically. Do not repeat it with `send_message`.
- Never output confirmations like "enviado", "mensagem enviada", "pronto" — your text output IS the reply.
- For tool calls, use strict JSON (no markdown fences or comments).

## Hub Skills

Skills `/hub-*` estão disponíveis para tarefas comuns. Antes de executar, considere delegar:

- **Pesquisa em código** → `/hub-explorar` (sub-agente leve)
- **Resumir algo extenso** → `/hub-resumir` (sub-agente leve)
- **Info do ambiente** → `/hub-info` (consulta rápida)
- **Portas disponíveis** → `/hub-portas`
- **Economia de recursos** → `/hub-recursos`
- **Gerar texto/copy** → `/hub-conteudo` (sub-agente)
- **Análise de dados** → `/hub-analisar` (sub-agente)
- **Revisão de código/texto** → `/hub-revisar` (sub-agente)
- **Criar plano** → `/hub-planejar` (salva em docs/)
- **Publicar site/mídia** → `/hub-publicar`
- **Interface do Hub** → `/hub-interface`
- **Estrutura do workspace** → `/hub-workspace`
- **MCPs disponíveis** → `/hub-mcps` (instalar, listar, entender)
- **Criar nova skill** → `/hub-criar-skill`

Delegue tarefas simples para sub-agentes. Economize recursos do agente principal para raciocínio complexo.

Current session topic: `eleva`
