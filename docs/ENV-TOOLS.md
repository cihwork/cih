IADev Desktop – Ferramentas e Diagnóstico do Host

Este guia resume utilitários e atalhos disponíveis dentro do ai-desktop para diagnosticar rede, portas e processos.

Diagnóstico de Rede e Portas
- iproute2: `ip`, `ss` – `ss -ltnp` para sockets/portas.
- lsof: `lsof -i :<porta>` – processos em uma porta.
- psmisc: `fuser`, `killall`, `pstree`.
- netcat-openbsd: `nc -zv host 80`.
- socat: utilitário de rede avançado.

Helpers
- `port-find <porta> [--udp]`
- `port-kill <porta> [--udp] [--signal SIGTERM|SIGKILL] [--dry-run]`

## Portas de Serviços Disponíveis

O ambiente oferece 20 slots de portas genéricas para seus serviços:

### Mapeamento de Portas por Ambiente
- **env-001**: portas 21001, 22001, 23001... até 40001 (slots 1-20)
- **env-002**: portas 21002, 22002, 23002... até 40002 (slots 1-20)  
- **env-XXX**: portas 21XXX, 22XXX, 23XXX... até 40XXX (slots 1-20)

### Como Usar
```bash
# Exemplo: rodar um servidor web no slot 1
podman run -d -p 21001:80 nginx:alpine

# Exemplo: rodar um app Node.js no slot 2  
podman run -d -p 22001:3000 my-node-app
```

### Verificar Portas Disponíveis
```bash
# Ver todas as portas do ambiente atual
./list.sh

# Verificar se uma porta específica está em uso
port-find 21001
```

Ferramentas Gerais
- jq, ripgrep (`rg`), fd (`fd`), bat (`bat`), tree, htop, strace, openssl.

Clientes de Serviços
- redis-tools (redis-cli): `redis-cli -h redis`

Mais detalhes: ver `/workspace/docs` deste arquivo no repositório (docs/ENV-TOOLS.md).
