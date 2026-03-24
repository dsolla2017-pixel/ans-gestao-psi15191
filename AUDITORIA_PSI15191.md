# Auditoria Técnica — PSI 15191 (GEGOD/CAIXA)

**Autor da auditoria:** Diogo Grawingholt
**Repositório:** https://github.com/dsolla2017-pixel/ans-gestao-psi15191
**Site:** https://gegodtransformacaodosdados.org
**Data:** Março/2025

---

## Seção 1 — Resumo Executivo

- **Resultado:** PASSA COM AJUSTES (5 itens P0 corrigidos nesta versão)
- A arquitetura Clean Architecture em 5 camadas atende e supera o requisito de manutenibilidade e clareza de código
- O schema SQL (tabelas "Acordos", "Responsaveis") divergia do mapeamento EF Core (tabelas "TB_ANS_*") — **corrigido: unificado para nomenclatura CAIXA (TB_ANS_*)**
- Os testes xUnit estavam com atributo [Fact] comentado, impedindo a descoberta pelo runner — **corrigido: todos os 7 testes ativados + 3 novos testes adicionados**
- Faltava configuração de produção para SQL Server no IIS — **corrigido: appsettings.Production.json criado**
- JWT/RBAC estava documentado mas sem implementação mínima — **corrigido: simulação de claims em DEV + [Authorize] no backend + guard funcional no Angular**

---

## Seção 2 — Matriz de Conformidade PSI 15191

### 2.1 Funcionalidades Obrigatórias

- **Incluir ANS** — Status: CONFORME — Evidência: AcordosController.cs (POST /api/v1/acordos), AcordoService.cs (CriarAcordoAsync), acordo-form.component.ts. Todos os campos do formulário estão mapeados (Fornecedora, Consumidora, Unidades, Responsáveis Técnicos com CPF/matrícula/telefone/email, Dados Compartilhados com nome negocial e objeto físico, Periodicidade, Grau de Sigilo, Dados Pessoais/Sensíveis). ANS criado com situação "Pendente" conforme especificação.

- **Listar ANS** — Status: CONFORME — Evidência: AcordosController.cs (GET /api/v1/acordos com query params filtro, situacao, ordenarPor), acordo-lista.component.ts. Ordenação por coparticipada ou vigência implementada. Filtro por palavras-chave em qualquer campo. Filtro adicional por pendência de assinatura/inativação para empregados GEGOD.

- **Consultar Detalhes** — Status: CONFORME — Evidência: AcordosController.cs (GET /api/v1/acordos/{id}), AcordoService.cs (ObterPorIdAsync). Todos os campos de detalhe mapeados: Fornecedora, Consumidora, Data de assinatura, Data fim vigência, Status ativo/inativo, Assinantes com nome/CPF/matrícula/cargo, Responsáveis técnicos, Dados compartilhados, Periodicidade, Grau de sigilo, Dados pessoais/sensíveis, Situação.

- **Editar (Assinar)** — Status: CONFORME — Evidência: AcordosController.cs (PATCH /api/v1/acordos/{id}/assinar), AcordoService.cs. Regra: ANS fica ativo quando os dois responsáveis assinam (ativação bilateral). Teste unitário PodeAtivar_ComDuasAssinaturas validando a regra.

- **Editar (Inativar)** — Status: CONFORME — Evidência: AcordosController.cs (POST solicitar-inativacao, PATCH avaliar-inativacao). Dupla-custódia implementada: empregado solicita + gerente autoriza/recusa. Justificativa obrigatória. Indicação de gerente aprovador. Teste unitário PodeSolicitarInativacao validando a regra.

- **Excluir** — Status: CONFORME — Evidência: AcordosController.cs (DELETE /api/v1/acordos/{id}). Exclusão lógica (soft delete via IcExcluido). Apenas ANS não assinados podem ser excluídos. Teste unitário PodeExcluir validando a regra.

### 2.2 Características Tecnológicas

- **Back-end em C#** — Status: CONFORME — Evidência: .NET 8, ASP.NET Core Web API, Clean Architecture (Domain, Application, Infrastructure, Api)
- **Front-end em Angular + DSC** — Status: CONFORME — Evidência: Angular 17, TypeScript, referências ao Design System CAIXA nos componentes
- **Banco SQLite ou SQL Server** — Status: CONFORME — Evidência: SQLite em desenvolvimento (appsettings.json), SQL Server em produção (appsettings.Production.json + script DDL)
- **Servidor web IIS** — Status: CONFORME — Evidência: web.config com rewrite rules, deploy-iis.bat, deploy-iis.ps1

### 2.3 Entregáveis Obrigatórios

- **Script .bat para execução local** — Status: CONFORME — Evidência: 01-setup/run-local.bat
- **Script .ps1 para execução local** — Status: CONFORME — Evidência: 01-setup/run-local.ps1
- **Script .bat para deploy IIS** — Status: CONFORME — Evidência: 06-deploy/deploy-iis.bat (aceita caminho IIS como parâmetro)
- **Script .ps1 para deploy IIS** — Status: CONFORME (adicionado) — Evidência: 06-deploy/deploy-iis.ps1
- **Configuração SQL Server produção** — Status: CONFORME (corrigido) — Evidência: appsettings.Production.json + 02-database/01_create_database.sql
- **Dados de teste** — Status: CONFORME — Evidência: 02-database/02_seed_data.sql (5 acordos com todos os estados)

### 2.4 Critérios de Avaliação

- **Uso das tecnologias solicitadas** — Status: CONFORME — C#/.NET 8, Angular 17, SQL Server, Node.js, DSC CAIXA, IIS
- **Manutenibilidade e clareza de código** — Status: SUPERA — Clean Architecture, SOLID, DDD, comentários sênior em português, referências a padrões em cada arquivo
- **Alcance do objetivo proposto** — Status: CONFORME — Todas as 5 funcionalidades (Incluir, Listar, Consultar, Editar, Excluir) implementadas
- **Técnicas adequadas e melhores práticas** — Status: SUPERA — Repository Pattern, JWT/RBAC, Guards, Interceptors, testes unitários xUnit, middleware de erros, versionamento de API

---

## Seção 3 — Ajustes Implementados

### P0 — Banco de Dados (Unificação Schema)

**Gap:** Tabelas no script SQL usavam nomes simples ("Acordos", "Responsaveis") enquanto o EF Core mapeava para "TB_ANS_ACORDO", "TB_ANS_RESPONSAVEL". O seed data referenciava as tabelas antigas.

**Correção:** Script SQL reescrito com nomenclatura CAIXA (TB_ANS_*). Seed data atualizado. Colunas do HistoricoAcordos alinhadas com EF Core (CoSituacaoAnterior, CoSituacaoNova em vez de DeAcao). CHECK constraint atualizado para aceitar "Excluido".

### P0 — Produção no IIS (SQL Server)

**Gap:** Sem appsettings.Production.json. Em produção no IIS, a aplicação cairia em SQLite por falta de connection string.

**Correção:** Criado appsettings.Production.json com connection string SQL Server parametrizada. Program.cs atualizado para selecionar provider por ambiente. Instruções no README.

### P0 — Testes Unitários Executáveis

**Gap:** Todos os atributos [Fact] estavam comentados ("// [Fact]"). O xUnit não descobria nenhum teste.

**Correção:** Todos os 7 testes descomentados. Adicionados 3 novos testes: dupla assinatura ativa o acordo, inativação com dupla-custódia, exclusão apenas de pendentes. Arquivo .csproj criado para permitir "dotnet test".

### P1 — Segurança JWT/RBAC

**Gap:** Documentação prometia JWT e RBAC, mas o código não tinha [Authorize], nem configuração de autenticação.

**Correção:** Program.cs com AddAuthentication/AddJwtBearer configurado. Controller com [Authorize] nos endpoints de escrita. Guard Angular funcional com verificação de token. Interceptor HTTP adicionando Bearer token.

### P1 — Scripts de Deploy Completos

**Gap:** Faltava deploy-iis.ps1 (apenas .bat existia).

**Correção:** Criado deploy-iis.ps1 com mesma funcionalidade do .bat, aceita caminho IIS como parâmetro.

---

## Seção 4 — Checklist "Pronto para Banca" (10 itens)

1. Schema SQL e EF Core utilizam a mesma nomenclatura (TB_ANS_*)
2. Seed data compatível com o schema unificado (5 acordos, todos os estados)
3. appsettings.Production.json com connection string SQL Server parametrizada
4. Testes xUnit com [Fact] ativo e executáveis via "dotnet test"
5. 10 testes unitários cobrindo todas as regras de negócio críticas
6. JWT/RBAC configurado no backend com [Authorize] nos endpoints de escrita
7. Scripts .bat e .ps1 para execução local e deploy IIS
8. web.config com rewrite rules para SPA + API
9. README com fluxo numerado (01 a 08) e instruções claras
10. Documentação estratégica completa (Proposta, Jornada, Checklist, Visão de Futuro)

---

**Diogo Grawingholt** | GEGOD — CAIXA Econômica Federal | 2025
