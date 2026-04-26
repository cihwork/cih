# Eleva — Rodar Testes e Validações

## Build

```bash
dotnet build Eleva.sln
```

## Testes

```bash
# Todos
dotnet test Eleva.sln

# Só unit tests
dotnet test tests/Eleva.UnitTests/

# Só integration tests
dotnet test tests/Eleva.IntegrationTests/

# Com output detalhado
dotnet test --logger "console;verbosity=normal"
```

## Health check manual

```bash
# Subir o servidor (porta 17001)
dotnet run --project src/Eleva.Server/Eleva.Server.csproj

# Health
curl http://localhost:17001/health

# Listar tools MCP
curl http://localhost:17001/mcp/tools -H "X-Instance-Id: 1"

# Chamar uma tool
curl -X POST http://localhost:17001/mcp/tools/call \
  -H "Content-Type: application/json" \
  -H "X-Instance-Id: 1" \
  -d '{"name":"system_health","arguments":{}}'

# Listar colaboradores
curl -X POST http://localhost:17001/mcp/tools/call \
  -H "Content-Type: application/json" \
  -H "X-Instance-Id: 1" \
  -d '{"name":"employee_list","arguments":{"take":10}}'
```

## Checklist de validação (spec)

- [ ] `dotnet build` sem erros
- [ ] `dotnet test` passando
- [ ] `GET /health` retorna `{"status":"healthy"}`
- [ ] `GET /mcp/tools` lista 67 tools de RH
- [ ] `POST /mcp/tools/call` com `employee_list` executa
- [ ] Migrations aplicadas no startup
- [ ] Nenhum artefato fora do domínio RH na solution
