# CLAUDE.md - Regras do Projeto EconomIA

## Regras de Deploy - CRÍTICO

### NUNCA fazer deploy direto em produção

**PROIBIDO:**
- Fazer `docker build` e `docker push` diretamente para imagens de produção
- Executar comandos `docker run/stop/rm` diretamente no servidor de produção
- Modificar banco de dados de produção diretamente

**OBRIGATÓRIO:**
- Todo código deve ser commitado e pushado para o repositório Git
- Deploy em produção SOMENTE via pipeline automático do Git (CI/CD)
- Testar SEMPRE em ambiente de desenvolvimento local antes de commitar
- Criar branch para features novas, fazer PR e merge para main

### Ambientes

- **DEV**: Máquina local com Docker Compose
- **PROD**: Servidor 136.113.233.79 - APENAS via deploy automático

### Fluxo de trabalho correto

1. Desenvolver e testar localmente
2. Commitar mudanças
3. Push para repositório remoto
4. Pipeline de CI/CD faz o deploy automático
5. Verificar logs e funcionamento em produção

## Estrutura do Projeto

- `EconomIA/` - API REST principal
- `EconomIA.CargaDeDados/` - Worker de carga de dados do PNCP
- `EconomIA.Domain/` - Entidades de domínio
- `EconomIA.Application/` - Casos de uso e queries
- `EconomIA.Adapters/` - Repositórios e adaptadores

## Banco de Dados

- PostgreSQL para dados relacionais
- Elasticsearch para busca de itens de compra
- Migrations via Entity Framework Core
