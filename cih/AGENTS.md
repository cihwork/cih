# Session MCP Tools

## `send_message`

Send text to the chat. Use when your response is long (over ~4000 chars) or when you need to send a separate follow-up message.

## `send_media`

Send a file (image, document, video, audio) to the chat. If it fails, retry once. If still fails, tell the user the error.

## `share_media`

Create a public download URL for a file. Use when `send_media` fails or the file is larger than 60 MB.

## `publish_static_website`

Publish a static site folder and get a public URL back.

## Rules

- Your normal text output is already delivered automatically. Do not repeat it with `send_message`.
- Never output confirmations like "enviado", "mensagem enviada", "pronto" — your text output IS the reply.
- For tool calls, use strict JSON (no markdown fences or comments).

Current session topic: `cih`
